using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFCustomMessageBox;
using ListBox = System.Windows.Controls.ListBox;
using TextBox = System.Windows.Controls.TextBox;

namespace LeHandUI
{
	
	public class SaveCommand : ICommand
	{
		public event EventHandler CanExecuteChanged;

		public SaveCommand() { }
		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			//MessageBox.Show("HelloWorld");
			string writePath = LHregistry.GetFile(FileManager.currentFileId);
			AdvancedMode.inst.textEditor.Save(writePath);
			AdvancedMode.UnChangedFile(AdvancedMode.Listbox);

		}
	}
	public class LineToggleCommand : ICommand
	{
		public event EventHandler CanExecuteChanged;

		public LineToggleCommand() { }
		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			AdvancedMode.inst.textEditor.ShowLineNumbers = !AdvancedMode.inst.textEditor.ShowLineNumbers;

		}
	}
	public partial class AdvancedMode: System.Windows.Controls.UserControl
	{
		public static AdvancedMode inst = null;
		public static System.Windows.Controls.ListBox Listbox = null;
		public static int SelectedItemIndex = -1;
		public static bool hasRefreshOccurredWithinSeconds = false;

		#region ImageSourceFromBitmap_func
		//Dit is mijn mooie gekopieerde stackoverflow code
		//If you get 'dllimport unknown'-, then add 'using System.Runtime.InteropServices;'
		[DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject([In] IntPtr hObject);

		public static ImageSource ImageSourceFromBitmap(Bitmap bmp)
		{
			var handle = bmp.GetHbitmap();
			try
			{
				return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			}
			finally { DeleteObject(handle); }
		}
        #endregion

        //Niek heeft al mooi een functie gemaakt die een string[] returnt met alle paths
        //DRIE FUNCTIES: ADD / REFERENCE FILE, REMOVE FILE, REFRESH FILES
        //List<string> LuaNames = new List<string>();



        #region environmentEvents

        private void FocusMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			//Debug.WriteLine(Keyboard.FocusedElement.ToString());
			UIElement UIE = (UIElement)sender;
			bool succes = UIE.Focus();

		}

		private void EditorBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			bool ctrl = (Keyboard.Modifiers == ModifierKeys.Control);
			if (ctrl)
			{
				UpdateFontSize(e.Delta > 0);
				e.Handled = true;
			}
		}

		// Reasonable max and min font size values
		private const double FONT_MAX_SIZE = 30d;
		private const double FONT_MIN_SIZE = 10d;

		// Update function, increases/decreases by a specific increment
		public void UpdateFontSize(bool increase)
		{
			double currentSize = textEditor.FontSize;

			if (increase)
			{
				if (currentSize < FONT_MAX_SIZE)
				{
					double newSize = Math.Min(FONT_MAX_SIZE, currentSize + 2);
					textEditor.FontSize = newSize;
				}
			}
			else
			{
				if (currentSize > FONT_MIN_SIZE)
				{
					double newSize = Math.Max(FONT_MIN_SIZE, currentSize - 2);
					textEditor.FontSize = newSize;
				}
			}
		}

		public void ChangeLabel(string label, int index)
		{
			LuaFileView.Items.RemoveAt(index);
			LuaFileView.Items.Insert(index, label);
		}
		public static void ChangeLabel(System.Windows.Controls.ListBox list, string label, int index)
		{
			list.Items.RemoveAt(index);
			list.Items.Insert(index, label);
		}
		public static void UnChangedFile(System.Windows.Controls.ListBox list)
		{
			int index = FileManager.currentLoadedIndex;
			if (index > -1 && FileManager.currentFileId > -1)
			{
				if (FileManager.isFileNotSaved[FileManager.currentFileId])
				{

					FileManager.isFileNotSaved[FileManager.currentFileId] = false;
					string label = (string)(list.Items[index]);
					label = label.Remove(label.Length - 1);
					ChangeLabel(list, label, index);
				}
			}
		}
		public bool BypassTextChangedEvent = true;
		private void ChangedFile(object sender, EventArgs e)
		{
			if (!BypassTextChangedEvent)
			{
				int index = FileManager.currentLoadedIndex;
				if (index > -1 && FileManager.currentFileId > -1)
				{
					if (!FileManager.isFileNotSaved[FileManager.currentFileId])
					{
						FileManager.isFileNotSaved[FileManager.currentFileId] = true;
						string label = (string)(LuaFileView.Items[index]) + "*";
						ChangeLabel(label, index);
					}
				}
			}
			BypassTextChangedEvent = false;
		}
        #endregion
        #region Button Click Events
		private void AddButton_Click(object sender, EventArgs e)
		{
			AddReferenceScript();
		}

        #endregion
        #region ButtonEvents
        private void AddReferenceScript()
		{
			Microsoft.Win32.OpenFileDialog openFileExplorer = new Microsoft.Win32.OpenFileDialog()
			{
				CheckFileExists = true,
				CheckPathExists = true,
				InitialDirectory = @"Documents",
				ShowReadOnly = true,
				Filter = " All files(*.*) 'cuz .lua doesn't work|*.*"
			};

			Nullable<bool> result = openFileExplorer.ShowDialog();
			if (result == true)
			{
				string newFilePath = openFileExplorer.FileName;
				int newFileId = FileManager.AddReference(newFilePath);
				LuaFileView.Items.Add(LHregistry.getSimpleName(newFilePath));

			}

		}
		private void RemoveLuaScript(object sender, EventArgs e)
		{
			int idToBeRemoved = -1; //some ridiculous number, i. e. -1 just isn't possible

			if (LuaFileView.SelectedIndex != -1)
			{

				idToBeRemoved = (LuaFileView.SelectedIndex);
				int[] allIds = LHregistry.GetAllFileIds();

				if (allIds.Length > idToBeRemoved && idToBeRemoved != -1)
				{
					LHregistry.RemoveFile(allIds[idToBeRemoved]);



					LuaFileView.Items.RemoveAt(idToBeRemoved);
				}
			}
		}
		private void SaveScript(object sender, EventArgs e)
		{
			string writePath = LHregistry.GetFile(FileManager.currentFileId);
				try
				{
					textEditor.Save(writePath);
				}
				catch (System.ArgumentException)
				{ Debug.WriteLine("Caught Argument Exception error, propably that the file can't be empty string, so yeah..."); }

				UnChangedFile(Listbox);
				return;
		}
		void LoadFileFromId(int index)
		{

			if (index < 0)
				return;

			//save old on memory
			if (FileManager.currentFileId >= 0)
				FileManager.FileCache[FileManager.currentFileId] = textEditor.Text;


			string FileContents = FileManager.LoadFile(index);
				if (FileContents != null)
				{
					BypassTextChangedEvent = true;
					textEditor.Text = FileContents;
				}
		} 

        private void LoadLuaFileFromSelectedObjectInList(object sender, EventArgs e)
		{
			System.Windows.Controls.ListBox naam = (System.Windows.Controls.ListBox)(sender);
			SelectedItemIndex = naam.SelectedIndex;
			if (SelectedItemIndex < 0)
				return;

			FileManager.currentLoadedIndex = SelectedItemIndex;


			int[] id = LHregistry.GetAllFileIds();
			int ActualFileId = id[SelectedItemIndex];

			LoadFileFromId(ActualFileId);
			LuaFileView.Items.Refresh();
			//ALWAYS REFRESH, saves some headaches, like trying to solve a nonexistent problem for two hours. Trust me, I know.

		}

		private void RefreshLuaScript(object sender, EventArgs e)
		{

			if (hasRefreshOccurredWithinSeconds == false)
			{ //if the refresh has not occurred in x milliseconds
				List<string> LuaNames = new List<string>(LHregistry.GetAllFilenames());
				for (int i = 0; i < LuaNames.Count; i++)
				{
					LuaNames.Add(LHregistry.getSimpleName(LuaNames[i]));
					LuaFileView.Items.Add(LuaNames[i]);
				}

			}
			else { }

			LuaFileView.Items.Refresh();
			hasRefreshOccurredWithinSeconds = true;
		}

		
		
		private void RunLuaScript(object sender, EventArgs e)
		{
			if (FileManager.currentFileId < 0)
				return;
			//check if files are saved
			bool unsavedFiles = false;
			for (int i = 0; i < FileManager.isFileNotSaved.Length; i++)
			{
				if (FileManager.isFileNotSaved[i])
				{
					unsavedFiles = true;
					break;
				}
			}
			if (unsavedFiles)
			{
				MessageBoxResult res = CustomMessageBox.ShowYesNoCancel(
					"There are unsaved files, do you want to save all files?",
					"Unsaved Files",
					"Yes", "No", "Cancel", MessageBoxImage.Exclamation
					);
				if (res == MessageBoxResult.Yes)
				{
					string writePath = LHregistry.GetFile(FileManager.currentFileId);
					textEditor.Save(writePath);
					UnChangedFile(Listbox);
				}
				if (res == MessageBoxResult.Cancel)
					return;

			}
			//load and run lua script

			Communicator.load(FileManager.files[FileManager.currentFileId]);
			Communicator.start();
			//start monitoring
			Startwindow sw = new Startwindow();
			sw.Show();

			LuaFileView.Items.Refresh();
			return;
		}
        #endregion
        #region Construction 
        
		public AdvancedMode()
        {
			InitializeComponent();
			FileManager.LoadAllFiles();
			inst = this;
			Focusable = true;

			StartupValues val = LHregistry.GetStartupValues();
			textEditor.FontSize = val.StartFontSize;
			textEditor.Text = "function Start()\n	print(\"preview\")\nend\n\n\n\n\n\n\n\n";
			LoadFileFromId(val.StartupFileId);
			
			textEditor.InputBindings.Add(
				new InputBinding(new SaveCommand(),
				new KeyGesture(Key.S, ModifierKeys.Control)

				));
			textEditor.InputBindings.Add(
				new InputBinding(new LineToggleCommand(),
				new KeyGesture(Key.L, ModifierKeys.Control)

				));
			textEditor.ShowLineNumbers = Convert.ToBoolean(val.ShowLineNumbers);
			
			//set file view to the listbox
			Listbox = LuaFileView;
			//get all the filenames from registry
			List<TextBox> LuaNames = new List<TextBox>(LHregistry.GetAllFilenames().Length);
			//display them
			for (int i = 0; i < LuaNames.Count; i++)
			{
				string wholePath = LHregistry.GetAllFilenames()[i];
				LuaNames[i].Text = LHregistry.getSimpleName(wholePath);
				LuaFileView.Items.Add(LuaNames[i]);
			}
			

            BypassTextChangedEvent = true;
			


			//Alle icoontjes
			PlusIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.PALE_GREEN_AddIcon64x64);
			DeleteIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.WASHED_OUT_RED_DeleteIcon64x64);
			//RefreshIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.AQUA_RefreshIcon64x64);
			//AddReferenceIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.AddReference16x16);
			SaveIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.SaveScript64x64);
			RunPrgmIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.StartScript64x64);
		}
		#endregion

		
	}
}

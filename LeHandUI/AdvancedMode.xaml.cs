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

namespace LeHandUI
{
	public class FirstDegreeFunctionConverter : IValueConverter
	{
		public double A { get; set; }

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			double a = GetDoubleValue(parameter, A);

			double x = GetDoubleValue(value, 0.0);

			return (a * x);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			double a = GetDoubleValue(parameter, A);

			double y = GetDoubleValue(value, 0.0);

			return (y) / a;
		}
		private double GetDoubleValue(object parameter, double defaultValue)
		{
			double a;
			if (parameter != null)
				try
				{
					a = System.Convert.ToDouble(parameter);
				}
				catch
				{
					a = defaultValue;
				}
			else
				a = defaultValue;
			return a;
		}
	}
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
			string writePath = LHregistry.GetFile(FileManager.currentFile);
			AdvancedMode.advancedInst.textEditor.Save(writePath);
			AdvancedMode.UnChangedFile(AdvancedMode.Listbox);

		}
	}
	
	public partial class AdvancedMode: System.Windows.Controls.UserControl
	{
		public static AdvancedMode advancedInst = null;
		public static System.Windows.Controls.ListBox Listbox = null;

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

	public void TextSizeChangeByAmount(MouseWheelEventArgs e)
		{
			double changeByAmount = 2;
				if (e.Delta < 0 && (Keyboard.IsKeyDown(Key.LeftCtrl)||Keyboard.IsKeyDown(Key.RightCtrl)))
				{
					textEditor.FontSize -= changeByAmount;
				}
				if (e.Delta > 0 && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
				{
					textEditor.FontSize += changeByAmount;
				}
		}

		Stopwatch refreshTimer = new Stopwatch();
		int SelectedItemIndex;
		bool hasRefreshOccurredWithinSeconds = false;

		private void LoadLuaFileFromSelectedObjectInList(object sender, EventArgs e)
		{
			System.Windows.Controls.ListBox naam = (System.Windows.Controls.ListBox)(sender);
			SelectedItemIndex = naam.SelectedIndex;
			if (SelectedItemIndex < 0)
				return;
			int[] id = LHregistry.GetAllFileIds();
			int ActualFileId = id[SelectedItemIndex];
			FileManager.currentLoadedIndex = SelectedItemIndex;

			//save old on memory
			if (FileManager.currentFile >= 0)
				FileManager.FileCache[FileManager.currentFile] = textEditor.Text;


			string FileContents = FileManager.LoadFile(ActualFileId);
			if (FileContents != null)
			{
				BypassTextChangedEvent = true;
				textEditor.Text = FileContents;
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

			//ALWAYS REFRESH, saves some headaches, like trying to solve a nonexistent problem for two hours. Trust me, I know.
			LuaFileView.Items.Refresh();

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

			hasRefreshOccurredWithinSeconds = true;
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
			if (index > -1 && FileManager.currentFile > -1)
			{
				if (FileManager.isFileNotSaved[FileManager.currentFile])
				{

					FileManager.isFileNotSaved[FileManager.currentFile] = false;
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
				if (index > -1 && FileManager.currentFile > -1)
				{
					if (!FileManager.isFileNotSaved[FileManager.currentFile])
					{
						FileManager.isFileNotSaved[FileManager.currentFile] = true;
						string label = (string)(LuaFileView.Items[index]) + "*";
						ChangeLabel(label, index);
					}
				}
			}
			BypassTextChangedEvent = false;
		}
		private void AddReferenceScript(object sender, EventArgs e)
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
		private void SaveScript(object sender, EventArgs e)
		{
			string writePath = LHregistry.GetFile(FileManager.currentFile);
			try
			{
				textEditor.Save(writePath);
			}
			catch (System.ArgumentException)
			{ Debug.WriteLine("Caught Argument Exception error, propably that the file can't be empty string, so yeah..."); }

			UnChangedFile(Listbox);
			return;
		}
		private void RunLuaScript(object sender, EventArgs e)
		{
			if (FileManager.currentFile < 0)
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
					string writePath = LHregistry.GetFile(FileManager.currentFile);
					textEditor.Save(writePath);
					UnChangedFile(Listbox);
				}
				if (res == MessageBoxResult.Cancel)
					return;

			}
			//load and run lua script

			Communicator.load(FileManager.files[FileManager.currentFile]);
			Communicator.start();
			//start monitoring
			Startwindow sw = new Startwindow();
			sw.Show();
			return;
		}

		void OnWindowLoaded(object sender, EventArgs e)
		{

			Communicator.Init();

		}		
	
		public AdvancedMode()
        {
			InitializeComponent();
			FileManager.LoadAllFiles();
			advancedInst = this;


			textEditor.InputBindings.Add(
				new InputBinding(new SaveCommand(),
				new KeyGesture(Key.S, ModifierKeys.Control)

				));

			
			//set file view to the listbox
			Listbox = LuaFileView;
			//get all the filenames from registry
			List<string> LuaNames = new List<string>(LHregistry.GetAllFilenames());
			//display them
			for (int i = 0; i < LuaNames.Count; i++)
			{
				LuaNames[i] = LHregistry.getSimpleName(LuaNames[i]);
				LuaFileView.Items.Add(LuaNames[i]);
			}

            BypassTextChangedEvent = true;
			textEditor.Text = "function Start()\n	print(\"preview\")\nend\n\n\n\n\n\n\n\n";


			//Alle icoontjes
			PlusIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.PALE_GREEN_AddIcon64x64);
			DeleteIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.WASHED_OUT_RED_DeleteIcon64x64);
			//RefreshIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.AQUA_RefreshIcon64x64);
			//AddReferenceIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.AddReference16x16);
			SaveIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.SaveScript64x64);
			RunPrgmIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.StartScript64x64);
		}

		}
}

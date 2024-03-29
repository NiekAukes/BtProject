﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFCustomMessageBox;
using Color = System.Windows.Media.Color;
using ContextMenu = System.Windows.Controls.ContextMenu;
using Cursors = System.Windows.Input.Cursors;
using Label = System.Windows.Controls.Label;
using ListBox = System.Windows.Controls.ListBox;

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
		static SolidColorBrush transparent				= new SolidColorBrush(Color.FromArgb( 0 , 100, 100, 100));
		static SolidColorBrush off_white				= new SolidColorBrush(Color.FromArgb(255, 242, 242, 242));
		static SolidColorBrush almostTransparentWhite	= new SolidColorBrush(Color.FromArgb(80 , 242, 242, 242));
		static SolidColorBrush halfTransparentWhite		= new SolidColorBrush(Color.FromArgb(140 , 242, 242, 242));
		static SolidColorBrush white					= new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
		static SolidColorBrush black					= new SolidColorBrush(Color.FromArgb(255,  0 ,  0 ,  0 ));
		static SolidColorBrush dark_blue				= new SolidColorBrush(Color.FromArgb(255,  40, 120, 200));
		static SolidColorBrush light_blue			    = new SolidColorBrush(Color.FromArgb(225, 110, 130, 255));
		static SolidColorBrush vague_purple				= new SolidColorBrush(Color.FromArgb(180, 160, 110, 255)); //nice purple

		public static AdvancedMode inst = null; //so that we can use inst.LuaFileView and such
		public static ListBox Listbox = null;
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

		#region BUTTON_HANDLERS + LABEL STYLES
		Label lastselectedLabel;

		private void StyleLuaListbox()
        {
			//
        }

		private void LuaFileView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
		}

		public static void styleLabel(Label label)
		{
			label.Foreground = off_white;
			label.Background = transparent;
			label.BorderBrush = dark_blue;
			label.BorderThickness = new Thickness(0);

			label.GotFocus += inst.Label_GotFocus;
			//label.MouseEnter += inst.Label_MouseEnter;
			//label.MouseLeave += inst.Label_MouseLeave;
			//label.MouseDown += inst.Label_MouseDown;

			if(label == inst.LuaFileView.SelectedItem)
            {
				styleOpenedFileLabel(label);
            }
			else if(inst.LuaFileView.Items.IndexOf(label) != -1 && inst.LuaFileView.Items.IndexOf(label) == inst.LuaFileView.SelectedIndex)
            {
				inst.styleSelectedLabel(label);
            }


			//Context menu creation
			ContextMenu lblMenu = new ContextMenu();
			Label item1 = new Label();
			lblMenu.Items.Add(item1);

			label.ContextMenu = lblMenu;
		}

		public static void styleOpenedFileLabel(Label label)
		{
			label.Foreground = white;
			label.Background = dark_blue;
			label.BorderBrush = dark_blue;
			label.BorderThickness = new Thickness(0);

			/*label.GotFocus += inst.Label_GotFocus;
			label.MouseEnter += inst.Label_MouseEnter;
			label.MouseLeave += inst.Label_MouseLeave;*/
		}
		public void styleSelectedLabel(Label label)
        {
			label.Foreground = white;
			label.Background = halfTransparentWhite;
			label.BorderBrush = light_blue;
			label.BorderThickness = new Thickness(0);
		}

		private void Label_GotFocus(object sender, RoutedEventArgs e)
		{
			//fucking douwe
			Label lelele = (Label)sender;
			if (lelele != LuaFileView.SelectedItem)
			{
				lelele.Foreground = white;
				lelele.Background = dark_blue;
				lelele.BorderBrush = off_white;
				lelele.BorderThickness = new Thickness(0);
			}
            else
            {
				Label_MouseDown(sender, e);
            }
		}
		
		private void Label_MouseEnter(object sender, RoutedEventArgs e)
		{
			Label lelele = (Label)sender;

			if (lelele != LuaFileView.SelectedItem) {
				lelele.Background = almostTransparentWhite;
				lelele.Foreground = white;
				lelele.BorderThickness = new Thickness(0);
			}
		}

		private void Label_MouseDown(object sender, RoutedEventArgs e)
		{
			Label lelele = (Label)sender;

			styleSelectedLabel(lelele);

			LuaFileView.SelectedItem = lelele;

			//Checks the whole list of items in the file viewer, this is to fix this bug:
			//-when you clicked on an item and then clicked another label, those two labels would both be styled as MouseDown (selected)
			//this loop fixes it by applying the normal style to all objects that are not the selected nor the opened file
			for (int i = 0; i < LuaFileView.Items.Count; i++)
			{
				if (LuaFileView.Items[i] != LuaFileView.SelectedItem)
				{
					if (i != FileManager.currentLoadedIndex)
					{
						Label bruhlabel = (Label)LuaFileView.Items[i];
						bruhlabel.Foreground = off_white;
						bruhlabel.Background = transparent;
						bruhlabel.BorderThickness = new Thickness(0);
					}
				}

				lastselectedLabel = lelele;
				}
			}	

		private void Label_MouseLeave(object sender, RoutedEventArgs e)
		{
			Label lelele = (Label)sender;
			if(lelele != LuaFileView.SelectedItem)
            {
				lelele.Foreground = white;
				lelele.Background = transparent;
				lelele.BorderThickness = new Thickness(0);
            }
		}

		#endregion

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

		

		public void ChangeLabelText(string label, int index)
		{
			LuaFileView.Items.RemoveAt(index);
			Label newLabel = new Label();
			newLabel.Content = label;

			styleLabel(newLabel);

			LuaFileView.Items.Insert(index, newLabel);
		}


		public static void ChangeLabelText(System.Windows.Controls.ListBox list, string label, int index)
		{
			list.Items.RemoveAt(index);
			Label newLabel = new Label();
			newLabel.Content = label;

			styleLabel(newLabel);

			list.Items.Insert(index, newLabel);
		}

		public static void UnChangedFile(System.Windows.Controls.ListBox list)
		{
			int index = FileManager.currentLoadedIndex;
			if (index > -1 && FileManager.currentFileId > -1)
			{
				if (FileManager.isFileNotSaved[FileManager.currentFileId])
				{

					FileManager.isFileNotSaved[FileManager.currentFileId] = false;
					Label textshitthingdinges = (Label)inst.LuaFileView.Items[index];

					string label = textshitthingdinges.Content.ToString();
					if (label[label.Length - 1] == '*')
					{
						label = label.Remove(label.Length - 1);
						textshitthingdinges.Content = label;
						ChangeLabelText(list, label, index);
					}
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

						Label fileshitthing = (Label)LuaFileView.Items[index];
						string label = fileshitthing.Content + "*";

						ChangeLabelText(label, index);
					}
				}
			}
			BypassTextChangedEvent = false;
		}
        #endregion

        #region ButtonEvents
        private void AddReferenceScript(object sender, EventArgs e)
		{
			Microsoft.Win32.OpenFileDialog openFileExplorer = new Microsoft.Win32.OpenFileDialog()
			{
				CheckFileExists = true,
				CheckPathExists = true,
				InitialDirectory = @"Documents",
				ShowReadOnly = true,
				Filter = "Lua Scripts (*.lua)|*.lua|All files (*.*)|*.*"
			};

			Nullable<bool> result = openFileExplorer.ShowDialog();
			if (result == true)
			{
				string newFilePath = openFileExplorer.FileName;
				int newFileId = FileManager.AddReference(newFilePath);

				//OLD	LuaFileView.Items.Add(LHregistry.getSimpleName(newFilePath));
				Label newLabelToAdd = new Label();
				newLabelToAdd.Content = LHregistry.getSimpleName(newFilePath);
				newLabelToAdd.Cursor = Cursors.Hand;


				styleLabel(newLabelToAdd);
				LuaFileView.Items.Add(newLabelToAdd);
			}

		}

		public static void Removeluasc(int id)
        {
			inst.LuaFileView.Items.RemoveAt(id);

			inst.LuaFileView.Items.Refresh();
		}

		private void RemoveLuaScript(object sender, EventArgs e)
		{
			int idToBeRemoved = -1; //some ridiculous number, i. e. -1 just isn't possible

			if (LuaFileView.SelectedIndex != -1)
			{
				//check if file is open
				idToBeRemoved = (LuaFileView.SelectedIndex);
				int[] allIds = LHregistry.GetAllFileIds();

				if (allIds.Length > idToBeRemoved && idToBeRemoved != -1)
				{
					LHregistry.RemoveFile(allIds[idToBeRemoved]);
					LuaFileView.Items.RemoveAt(idToBeRemoved);
					
					LuaFileView.Items.Refresh();
				}
                
				RefreshLuaScript();
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
			ListBox lstbx = (ListBox)(sender);
			SelectedItemIndex = lstbx.SelectedIndex;
			if (SelectedItemIndex < 0)
				return;

			FileManager.currentLoadedIndex = SelectedItemIndex;


			int[] id = LHregistry.GetAllFileIds();
			int ActualFileId = id[SelectedItemIndex];

			LoadFileFromId(ActualFileId);
			LuaFileView.Items.Refresh();
			//ALWAYS REFRESH, saves some headaches, like trying to solve a nonexistent problem for two hours. Trust me, I know.

			RefreshLuaScript();
		}

		private void RefreshLuaScript()
		{
			List<string> LuaNames = new List<string>(LHregistry.GetAllFilenames());

			int currOpenedFileId = FileManager.currentLoadedIndex;


			for (int i = 0; i < LuaNames.Count; i++)
			{
				
				Label currLuaFileViewLabel = (Label)(LuaFileView.Items[i]);
				
				if (LuaNames[i] == (string)currLuaFileViewLabel.Content)
				{
					continue;
				}
				else
				{
					Label label = new Label();
					label.Name = "TxtBox" + i.ToString();
					label.Content = LHregistry.getSimpleName(LuaNames[i]);

					if (FileManager.isFileNotSaved[i])
						label.Content += "*";

					if (currOpenedFileId == i)
					{
						styleOpenedFileLabel(label);
					}
					else
					{
						styleLabel(label);
					}
					LuaFileView.Items.RemoveAt(i);
					LuaFileView.Items.Insert(i,label);


				}
			}
			LuaFileView.Items.Refresh();
		}

		public void SaveTextEditor()
        {
			string writePath = LHregistry.GetFile(FileManager.currentFileId);
			textEditor.Save(writePath);
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

            LuaFileView.SelectionChanged += LuaFileView_SelectionChanged;
			FileManager.LoadAllFiles();

			inst = this;
			Focusable = true;

			StartupValues val = LHregistry.GetStartupValues();
			//SimpleMode.LastFileOpened = val.LastSimpleFileSelected;
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
			StyleLuaListbox();

			//get all the filenames from registry
			int len = LHregistry.GetAllFilenames().Length;
			for (int i = 0; i < len; i++)
			{
				Label txtbox = new Label();
				string wholePath = LHregistry.GetAllFilenames()[i];
				txtbox.Content = LHregistry.getSimpleName(wholePath);

				styleLabel(txtbox);
				txtbox.Foreground = white;
				txtbox.Background = transparent;

				LuaFileView.Items.Add(txtbox);
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

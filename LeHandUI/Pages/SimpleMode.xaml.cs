using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using Cursors = System.Windows.Input.Cursors;
using Label = System.Windows.Controls.Label;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

/*COLOUR SCHEME: #212121 , #065464 ,  #34acbc ,     #85c3cf ,      #7a7d84 
                   (33,33,33),   (6,84,100),(52,172,188), (133,195,207), (122,125,132)
                 very dark blue, dark blue, aqua, pale aqua,     greyish
*/

namespace LeHandUI
{
    public partial class SimpleMode : System.Windows.Controls.UserControl
	{
		public static string[] fileNames = SimpleFileManager.FileNames();
		public static List<UIElement> listBoxUIElemtents = new List<UIElement>();

		public List<FileData> CurrentLoadedFileData = new List<FileData>();

		static SolidColorBrush transparent = new SolidColorBrush(Color.FromArgb(0, 100, 100, 100));
		static SolidColorBrush off_white = new SolidColorBrush(Color.FromArgb(255, 242, 242, 242));
		static SolidColorBrush almostTransparentWhite = new SolidColorBrush(Color.FromArgb(80, 242, 242, 242));
		static SolidColorBrush halfTransparentWhite = new SolidColorBrush(Color.FromArgb(140, 242, 242, 242));
		static SolidColorBrush white = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
		static SolidColorBrush black = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
		static SolidColorBrush dark_blue = new SolidColorBrush(Color.FromArgb(255, 40, 120, 200));
		static SolidColorBrush light_blue = new SolidColorBrush(Color.FromArgb(225, 110, 130, 255));
		static SolidColorBrush vague_purple = new SolidColorBrush(Color.FromArgb(180, 160, 110, 255)); //nice purple

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

        #region Construction/Initialisation
        public SimpleMode()
		{
			InitializeComponent();
		
			addFileImage.Source		=	ImageSourceFromBitmap(Properties.Resources.AddFile64x64);
			removeFileImage.Source	=	ImageSourceFromBitmap(Properties.Resources.RemoveFile64x64);
			addRuleImage.Source		=	ImageSourceFromBitmap(Properties.Resources.PALE_GREEN_AddIcon64x64);
			removeRuleImage.Source	=	ImageSourceFromBitmap(Properties.Resources.WASHED_OUT_RED_DeleteIcon64x64);

			simpleModeFileListBox.ItemsSource = listBoxUIElemtents;

            /* //DATA FOR TEST FILES
			FileData[] data = new FileData[6];
			FileData[] newdata = new FileData[1];
			newdata[0] = new FileData(0, 0.2, 0.8, 0, 30, 0);
			SimpleFileManager.ChangeFile("kiekoek", newdata);
			SimpleFileManager.ChangeFile("halloe", newdata);*/

            //INITIALIZE THE TEXTBOXARRAY, adds all textboxes of files to the textBoxes array so that we can check for not changed files and shit
            for (int i = 0; i < fileNames.Length; i++)
            {
				Label lbl = new Label();
				StyleNonSelectedLabel(lbl);
				lbl.Content = fileNames[i];

				listBoxUIElemtents.Add(lbl);
            }

			refreshFiles();

			//getfiledata returns null, pls fix volvo
			FileData dat = SimpleFileManager.GetFileData(0)[0];
			
		}
        #endregion

        private string prevname = "something wrong";


		public void refreshFiles()
		{
			fileNames = null;
			fileNames = SimpleFileManager.FileNames();	//returns all "Simple" file names by splitting the name in the '.' 
														//thus only getting the name, not the extension (ex: .lua gets deleted)

			//NIEK MOET DIT NOG GAAN IMPLEMENTEREN
			int currentOpenedFile = -1;//SimpleFileManager.currentLoadedIndex DOES NOT EXIST YET

			for (int i = 0; i < fileNames.Length; i++)
			{
				TextBox listboxTextBox = null;
				Label listboxLabel = null;

				while(listBoxUIElemtents.Count < fileNames.Length) //to make the length of both arrays identical (well not arrays, a list and an array but fuck it)
                {
					Label newemptylabel = new Label();
					listBoxUIElemtents.Add(newemptylabel);
                }

				if (listBoxUIElemtents[i] != null) {
					try
					{
						listboxLabel = (Label)listBoxUIElemtents[i];
					}
					catch (Exception e) {
						listboxTextBox = (TextBox)listBoxUIElemtents[i];
						Debug.WriteLine("Caught exception (file is textbox, not label): " + e);
					}
				}

				if (listboxTextBox != null || listboxLabel != null)
				{
					
					if (listBoxUIElemtents[i] != null || (String)((Label)(listBoxUIElemtents[i])).Content == fileNames[i])
					{
						continue;
					}
					else
					{
						Label lbl = new Label();
						lbl.Name = "txtbx " + i.ToString();
						lbl.Content = fileNames[i];

						if (currentOpenedFile == i)
						{
							StyleOpenedLabel(lbl);
						}

						else
						{
							StyleNonSelectedLabel(lbl);
						}

						listBoxUIElemtents.RemoveAt(i);
						listBoxUIElemtents.Insert(i, lbl);

					}
				}
			}

			simpleModeFileListBox.Items.Refresh();
		}
        /* HERE FOR REWRITING PURPOSES
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

					if (currOpenedFileId == i)
					{
						styleOpenedFileLabel(label);
					}
					else
					{
						styleLabel(label);
					}
					LuaFileView.Items.RemoveAt(i);
					LuaFileView.Items.Insert(i, label);
				}
			}
			LuaFileView.Items.Refresh();
		}*/

        #region LoadnSave
		public void LoadFile(int selectedindex)
        {
			CurrentLoadedFileData = new List<FileData>(SimpleFileManager.GetFileData(selectedindex));
			//Clear stackpanel van parametershit
			parameterPanel.Children.Clear();
			//for alle filedata in de file, maak nieuwe parametereditor.xaml en vul alles in de parametereditor xaml in
			for (int i = 0; i < CurrentLoadedFileData.Count; i++)
            {
				simpleModeParameterEditor neweditor = new simpleModeParameterEditor();
				
				parameterPanel.Children.Add();
            }

        }
		public void SaveChanges()
        {
			//pak alle instellingen van parametereditor xaml user interfaces
			//gooi ze in een filedata list
			//parse de filedata nar simplemodefilemanager
			//SimpleFileManager.ChangeFile() dus
        }
        #endregion

        #region StyleFunctions

        public void StyleNonSelectedLabel(Label lbl)
        {
			lbl.Focusable = true;

			//txtbox.Cursor = Cursors.Arrow;

			lbl.Background = transparent;
			lbl.Foreground = off_white;
			lbl.BorderThickness = new Thickness(0);
			lbl.BorderBrush = transparent;

			lbl.LostKeyboardFocus	+= Lbl_LostKeyboardFocus;
			lbl.PreviewMouseLeftButtonDown	+= Lbl_MouseDown;
            lbl.MouseEnter			+= Lbl_MouseEnter;
			lbl.MouseLeave			+= Lbl_MouseLeave;
		}

		public void styleMouseOverLabel(Label lbl)
        {
			lbl.Cursor = Cursors.Hand;

			if (lbl != simpleModeFileListBox.SelectedItem)
			{
				lbl.Background = almostTransparentWhite;
				lbl.Foreground = white;
			}
			else if(lbl == simpleModeFileListBox.SelectedItem)
            {
				StyleSelectedLabel(lbl);
            }

		}

		public void StyleSelectedLabel(Label lbl)
        {
			lbl.Cursor = Cursors.Hand;
			if (lbl == simpleModeFileListBox.SelectedItem)
			{
				lbl.Background = white;
				lbl.Foreground = black;
				lbl.BorderThickness = new Thickness(0);
				lbl.BorderBrush = white;
			}
            else if(lbl != simpleModeFileListBox.SelectedItem)
            {
				StyleNonSelectedLabel(lbl);
            }

		}

		public void StyleOpenedLabel(Label lbl)
        {
			lbl.Cursor = Cursors.Hand;

			lbl.Foreground = white;
			lbl.Background = dark_blue;
			lbl.BorderBrush = dark_blue;
			lbl.BorderThickness = new Thickness(0);


			for (int i =0; i < simpleModeFileListBox.Items.Count; i++)
            {
				if(simpleModeFileListBox.Items[i] != simpleModeFileListBox.SelectedItem)
                {
					StyleNonSelectedLabel((Label)(simpleModeFileListBox.Items[i]));
                }
            }
		}

        #endregion

        #region saveFileFunc!MOET NOG SPUL GEBEUREN AUKES
        public void saveCurrentFile(string name)
		{
			//get all rules from current file
			int selectedIndex = simpleModeFileListBox.SelectedIndex;
			if (selectedIndex != -1)
			{
				//IList<FileData> currentFileData = SimpleFileManager.GetFileData(selectedIndex);
				IList<FileData> dat = new List<FileData>();

				SimpleFileManager.ChangeFile(name, dat);
			}

			//fix the rest lazy piece of shit douwe

		}
		public void saveCurrentFile()
		{
			//get all rules from current file
			int selectedIndex = simpleModeFileListBox.SelectedIndex;
			if (selectedIndex != -1)
			{
				//IList<FileData> currentFileData = SimpleFileManager.GetFileData(selectedIndex);
				IList<FileData> dat = new List<FileData>();

				//simpleModeParameterEditor[] smpe = 

				SimpleFileManager.ChangeFile((string)((Label)(simpleModeFileListBox.SelectedItem)).Content, dat);
			}

			//fix the rest lazy piece of shit douwe

		}
        #endregion

        #region Label/txtbox Handlers
        private void Txtbox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				LabelOrTxtboxLostFocus(sender);
			}
		}
		private void Lbl_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			LabelOrTxtboxLostFocus(sender);
			
		}

		private void Lbl_MouseEnter(object sender, MouseEventArgs e) //voor style
        {
			Label lbl = (Label)sender;

			if (lbl != simpleModeFileListBox.SelectedItem)
			{
				styleMouseOverLabel(lbl);
			}
        }

		private void Lbl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) //ook puur voor stijl
		{
			MainWindow.inst.Focus();

			if (sender != simpleModeFileListBox.SelectedItem)
			{
				Label lbl = (Label)sender;
				lbl.Background = transparent;
				lbl.Foreground = off_white;
				lbl.BorderThickness = new Thickness(0);
				lbl.BorderBrush = transparent;
			}

		}

		public Label prevlbl = null;
		private void Lbl_MouseDown(object sender, MouseEventArgs e)
		{
			Label lbl = (Label)sender;
			
			simpleModeFileListBox.SelectedItem = lbl;
			
			if (lbl != prevlbl) {
				StyleSelectedLabel(lbl);
			}
			if (prevlbl != null)
            {
				StyleNonSelectedLabel(prevlbl);
            }
			prevlbl = lbl;

			//Open the file:
		}

		private void LabelOrTxtboxLostFocus(object sender) //if the text is altered in the textbox, replace the name in the filemanager with the textbox
		{
			Label lbl = (Label)sender;

			if (lbl != simpleModeFileListBox.SelectedItem) //werkt dit? geen flauw idee, vast wel
			{
				StyleNonSelectedLabel(lbl);
			}

			if (((string)(lbl.Content)).Length > 1)
			{
				SimpleFileManager.ChangeName(prevname, (string)lbl.Content);
				refreshFiles();
			}

			else
			{
				lbl.Content = prevname;

				int index = simpleModeFileListBox.Items.IndexOf(sender);
				listBoxUIElemtents.RemoveAt(index);
				listBoxUIElemtents.Insert(index, lbl);
			}
		}

		#endregion

		#region ListBox Handlers

		public static int LastFileOpened = 0xFFFF;
        private void onTextChanged(object sender, TextChangedEventArgs e)
		{
			/*TextBox listitem = (TextBox)sender;
			int index = simpleModeFileListBox.SelectedIndex;
			if (index != -1)
			{
				SimpleFileManager.ChangeName(fileNames[index], listitem.Content);
				refreshFiles();
				dosumShit(sender);
			}*/
		}
		/*private void onListItemSelected(object sender, System.Windows.Input.MouseEventArgs e)
		{
			TextBox listitem = (TextBox)sender;
			listitem.Focus();

			listitem.Background			= dark_blue;
			listitem.Foreground			= white;
			listitem.BorderThickness	= new Thickness(0);

		}
		private void onListItemLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			MainWindow.inst.Focus();

			TextBox listitem = (TextBox)sender;
			listitem.Background = transparent;
			listitem.Foreground = off_white;
			listitem.BorderThickness = new Thickness(0);
			listitem.BorderBrush = transparent;
		}*/
		#endregion

		#region Button Handlers

		public int totaladdedfiles = 0;
        private void addFileButton_Click(object sender, RoutedEventArgs e)
		{
			//add a filename and ruleset
			FileData[] emptyfiledata = new FileData[1];
			string newFileName = "New file " + totaladdedfiles.ToString();
			SimpleFileManager.ChangeFile(newFileName, emptyfiledata);
			totaladdedfiles++;

			refreshFiles();
			saveCurrentFile();
			simpleModeFileListBox.Focus();
		}
		private void removeFileButton_Click(object sender, RoutedEventArgs e)
		{
			int selectedItemIndex = simpleModeFileListBox.SelectedIndex;
			if (selectedItemIndex != -1)
			{
				SimpleFileManager.DeleteFile(selectedItemIndex);
			}
			refreshFiles();
		}

		private void addRuleButton_Click(object sender, RoutedEventArgs e)
		{
            //add a rule to the ruleset file:
            //naam character: 0x00 + character voor variabele + 2 doubles + character voor action id + 2 keer 4 bytes voor args
            //voorbeeld: 0x01 (als variabele in deze range zit) + 0x01 (één v.d. vingers, xyz as van acceleratie of rotatie) + 0x03 (...) + arg1 t/m arg4
            //voorbeeld: 0x01 0x01 0x03 0x45 0x74 0x19 0x20

            //Moet een usercontrol invoegen (simplemodeparametereditor.xaml) aan de stackpanel in simplemode
            //moet naam geven denk ik
            List <simpleModeParameterEditor> parameterEditorList = new List<simpleModeParameterEditor>();

			int selectedIndex = simpleModeFileListBox.SelectedIndex; //loopen door de filedata en alles in parameter editor ui zetten
			 SimpleFileManager.GetFileData(selectedIndex);


			saveCurrentFile();
		}
		private void removeRuleButton_Click(object sender, RoutedEventArgs e)
		{
			//remove the shit above

			saveCurrentFile();
		}
		#endregion

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void simpleModeFileListBox_MouseDown(object sender, MouseButtonEventArgs e)
		{
			saveCurrentFile();

		}
	}
}

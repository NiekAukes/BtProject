using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
    public partial class SimpleMode : UserControl
	{
		public static string[] fileNames = SimpleFileManager.FileNames();
		public static List<UIElement> listBoxUIElemtents = new List<UIElement>();

		public List<FileData> CurrentLoadedFileData = new List<FileData>();

		FileSystemWatcher FSW = null;

		

		bool fAlreadySaving = false;
		int lastSelectedFileIndex = -1;
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
			
			addFileImage.Source = ImageSourceFromBitmap(Properties.Resources.AddFile64x64);
			removeFileImage.Source = ImageSourceFromBitmap(Properties.Resources.RemoveFile64x64);
			renameFileImage.Source = ImageSourceFromBitmap(Properties.Resources.RenameFile64x64);
			addRuleImage.Source = ImageSourceFromBitmap(Properties.Resources.PALE_GREEN_AddIcon64x64);
			removeRuleImage.Source = ImageSourceFromBitmap(Properties.Resources.WASHED_OUT_RED_DeleteIcon64x64);
			StartButtonImage.Source = ImageSourceFromBitmap(Properties.Resources.StartScript64x64);

			simpleModeFileListBox.ItemsSource = listBoxUIElemtents;
            simpleModeFileListBox.SelectionChanged += SimpleModeFileListBox_SelectionChanged;
			parameterPanel.Items.Clear();

			#region File System Watcher Code
			FSW = new FileSystemWatcher(MainWindow.Directory + "\\Files");

			FSW.NotifyFilter = NotifyFilters.LastAccess
							 | NotifyFilters.LastWrite
							 | NotifyFilters.FileName
							 | NotifyFilters.DirectoryName;

			FSW.Changed += FSW_Changed;
			FSW.Deleted += FSW_Changed;
			FSW.Created += FSW_Changed;
			FSW.Renamed += FSW_Changed;

			FSW.EnableRaisingEvents = true;
            # endregion

            refreshFiles();
		}

        
        #endregion

        private string prevname = "something wrong";


		public void refreshFiles()
		{
			listBoxUIElemtents.Clear();
			fileNames = SimpleFileManager.FileNames();  //returns all "Simple" file names by splitting the name in the '.' 
														//thus only getting the name, not the extension (ex: .lua gets deleted)

			//NIEK MOET DIT NOG GAAN IMPLEMENTEREN
			int currentOpenedFile = -1;//SimpleFileManager.currentLoadedIndex DOES NOT EXIST YET

			for (int i = 0; i < fileNames.Length; i++)
			{
				TextBox listboxTextBox = new TextBox();
				Label listboxLabel = new Label();
				listboxLabel.Content = fileNames[i];
				listboxTextBox.Text = fileNames[i];
				//listboxTextBox.IsReadOnly = false;
				listboxTextBox.Background = new SolidColorBrush(Colors.Transparent);
				listboxTextBox.Foreground = System.Windows.Media.Brushes.White;
				listboxTextBox.IsHitTestVisible = false;
				listboxTextBox.BorderBrush = System.Windows.Media.Brushes.Transparent;
				listboxTextBox.Margin = new Thickness(0, 3, 0, 3);

				listBoxUIElemtents.Add(listboxTextBox);

			}


			//FSW = new FileSystemWatcher(MainWindow.Directory + "\\Files");

			//FSW.NotifyFilter = NotifyFilters.LastAccess
			//				 | NotifyFilters.LastWrite
			//				 | NotifyFilters.FileName
			//				 | NotifyFilters.DirectoryName;

			//FSW.Changed += FSW_Changed;
			//FSW.Deleted += FSW_Changed;
			//FSW.Created += FSW_Changed;
			//FSW.Renamed += FSW_Changed;

			//FSW.EnableRaisingEvents = true;
			simpleModeFileListBox.Items.Refresh();
		}

        private void FSW_Changed(object sender, FileSystemEventArgs e)
        {
			Application.Current.Dispatcher.Invoke((Action)delegate {
				refreshFiles();
				Debug.WriteLine(e.ChangeType);
			});
        }
        public void LoadFile(int selectedindex)
        {
			if (!fAlreadySaving)
			{
				fAlreadySaving = true;
				IEnumerable<FileData> data = SimpleFileManager.GetFileData(selectedindex);
				if (data == null)
					return;
				CurrentLoadedFileData = new List<FileData>(data);
				//Clear stackpanel van parametershit
				parameterPanel.Items.Clear();
				for (int i = 0; i < CurrentLoadedFileData.Count; i++)
				{
					simpleModeParameterEditor parEdit = CurrentLoadedFileData[i].toSMPE();//for alle filedata in de file, maak nieuwe parametereditor.xaml en vul alles in de parametereditor xaml in
                    Grid.SetColumn(parEdit, 3);

					parameterPanel.Items.Add(parEdit);
				}
				fAlreadySaving = false;
			}

        }

        public void SaveChanges(string filename)
        {
			//pak alle instellingen van parametereditor xaml user interfaces
			//gooi ze in een filedata list
			//parse de filedata nar simplemodefilemanager
			//SimpleFileManager.ChangeFile() dus
			if (!fAlreadySaving)
			{
				fAlreadySaving = true;
				List<FileData> savedData = new List<FileData>();
				for (int i = 0; i < parameterPanel.Items.Count; i++)
				{
					simpleModeParameterEditor currEditor = (simpleModeParameterEditor)parameterPanel.Items[i];
					FileData newFileData = new FileData();
					newFileData.variable = (byte)currEditor.varChooser.SelectedIndex;
					newFileData.beginRange = currEditor.lowerSlider.Value;
					newFileData.endRange = currEditor.upperSlider.Value;
					newFileData.actionId = (byte)currEditor.actionChooser.SelectedIndex;

					switch (currEditor.actionChooser.SelectedIndex)
					{
						case 0:
							try
							{
								newFileData.arg1 = ASCII_table.ascii_table[currEditor.KeyPressChooser.Text.ToUpper().Trim(new char[] { '\u007f', '\0' } )];//(char)currEditor.KeyPressChooser.Text;

							}
							catch (KeyNotFoundException)
							{
								Console.WriteLine("key could not be found in dictionary ascii_table");
								//SHOW POPUP THAT SAVING COULD NOT BE COMPLETED

							}

							break;
						case 1: //keypress (insert selected index of mousepresschooser into arg1)
							newFileData.arg1 = currEditor.MousePressChooser.SelectedIndex;
							break;

						case 2: //mousemove (insert arg1 and arg2 with mouse1 and mouse2

							//CONVERTS STRINGS INTO LONGS, typing a character into the textbox is prevented in the textbox handlers.
							//die longs werken voor geen kut
							long ret = 0, ret2 = 0;
							if (long.TryParse(currEditor.MouseMoveBox1.Text, out ret))
								newFileData.arg1 = ret;
							if (long.TryParse(currEditor.MouseMoveBox2.Text, out ret2))
								newFileData.arg2 = ret2;

							break;
						case 3:
							//do nothing because the program exit does not require any arguments
							break;
					}

					savedData.Add(newFileData);
				}
				fAlreadySaving = false;

				//save the files
				SimpleFileManager.ChangeFile(filename, savedData);
			}
        }

        #region StyleFunctions

        public void StyleNonSelectedLabel(TextBox lbl)
        {
			lbl.Focusable = true;

			//txtbox.Cursor = Cursors.Arrow;

			lbl.Background = transparent;
			lbl.Foreground = off_white;
			lbl.BorderThickness = new Thickness(0);
			lbl.BorderBrush = transparent;

			lbl.LostKeyboardFocus	+= Lbl_LostKeyboardFocus;
			lbl.PreviewMouseLeftButtonUp	+= LblMouseUp;
			lbl.MouseLeftButtonUp += LblMouseUp;
            lbl.MouseEnter			+= Lbl_MouseEnter;
			lbl.MouseLeave			+= Lbl_MouseLeave;

			//lbl.Style = this.FindResource("labelSimpleModeStyle") as Style;

		}

		public void styleMouseOverLabel(TextBox lbl)
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

		public void StyleSelectedLabel(TextBox lbl)
        {
			lbl.Cursor = Cursors.Hand;
			if (lbl == simpleModeFileListBox.SelectedItem)
			{
				lbl.Background = transparent;
				lbl.Foreground = white;
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
					StyleNonSelectedLabel((TextBox)(simpleModeFileListBox.Items[i]));
                }
            }
		}

		#endregion

		#region saveFileFunc!
		private void SimpleModeFileListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			TextBox lbl = (TextBox)simpleModeFileListBox.SelectedItem;

			if (lbl != prevlbl && lbl != null)
			{
				if (prevlbl != null)
				{
					if (File.Exists(MainWindow.Directory + "\\Files\\" + prevlbl.Text + ".lh"))
					{
						StyleNonSelectedLabel(prevlbl);

						SaveChanges(prevlbl.Text);
					}
				}
				lastSelectedFileIndex = simpleModeFileListBox.SelectedIndex;
				StyleSelectedLabel(lbl);

				LoadFile(lastSelectedFileIndex);

				prevlbl = lbl;

			}


		}
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
			if (simpleModeFileListBox.SelectedIndex != -1)
			{
				//IList<FileData> currentFileData = SimpleFileManager.GetFileData(selectedIndex);
				IList<FileData> dat = new List<FileData>();


				SimpleFileManager.ChangeFile((string)((TextBox)(simpleModeFileListBox.SelectedItem)).Text, dat);
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
			TextBox lbl = (TextBox)sender;

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
				
				StyleNonSelectedLabel((TextBox)sender);
			}

		}

		public TextBox prevlbl = null;
		private async void LblMouseUp(object sender, MouseEventArgs e)
		{
			
			TextBox lbl = (TextBox)sender;
			
			simpleModeFileListBox.SelectedItem = lbl;
			
			if (lbl != prevlbl) {
				if (prevlbl != null)
				{
					StyleNonSelectedLabel(prevlbl);

					SaveChanges(lbl.Text);
				}
				StyleSelectedLabel(lbl);

				//skips loading or saving unless cooldown of 100ms is finished.

				LoadFile(simpleModeFileListBox.SelectedIndex);
					
			}
			
			prevlbl = lbl;

			//Open the file:
		}
		
		private void LabelOrTxtboxLostFocus(object sender) //if the text is altered in the textbox, replace the name in the filemanager with the textbox
		{
			TextBox lbl = (TextBox)sender;

			if (lbl != simpleModeFileListBox.SelectedItem) //werkt dit? geen flauw idee, vast wel
			{
				StyleNonSelectedLabel(lbl);
			}

			if (((string)(lbl.Text)).Length > 1)
			{
				SimpleFileManager.ChangeName(prevname, (string)lbl.Text);
				refreshFiles();
			}

			else
			{
				lbl.Text = prevname;

				int index = simpleModeFileListBox.Items.IndexOf(sender);
				listBoxUIElemtents.RemoveAt(index);
				listBoxUIElemtents.Insert(index, lbl);
			}
		}

		#endregion

		#region Button Handlers
		string txtbxname;
		private void RenameFileButton_Click(object sender, RoutedEventArgs e)
		{
			int indextobechanged;
			if (simpleModeFileListBox.SelectedIndex == -1)
			{
				if (lastSelectedFileIndex == -1)
					return;
				else
					indextobechanged = lastSelectedFileIndex;
			}
            else
            {
				indextobechanged = simpleModeFileListBox.SelectedIndex;
            }
			UIElement selElement = listBoxUIElemtents[indextobechanged];

			if (selElement is TextBox)
			{
				TextBox selTxtBx = (TextBox)selElement;
				txtbxname = selTxtBx.Text;
				selTxtBx.Focus(); selTxtBx.IsEnabled = true; selTxtBx.IsReadOnly = false;

				selTxtBx.LostFocus += SelTxtBx_LostFocus;
			}
		}
		private void SelTxtBx_LostFocus(object sender, RoutedEventArgs e)
		{
			//make it non editable again
			TextBox SelTxtBx = (TextBox)sender; SelTxtBx.IsEnabled = false; SelTxtBx.IsReadOnly = true;
			SimpleFileManager.ChangeName(txtbxname, SelTxtBx.Text);

		}

		public int fileIndexCount = 0;
        private void addFileButton_Click(object sender, RoutedEventArgs e)
		{
			//add a filename and ruleset
			FileData[] emptyfiledata = new FileData[1];

			/*
			while (true)
			{
				string newFileName = "New file " + fileIndexCount.ToString();
				if (!SimpleFileManager.FileExists(newFileName))
				{
					SimpleFileManager.ChangeFile(newFileName, emptyfiledata);
					break;
				}
				else fileIndexCount++;
			}
			fileIndexCount++;*/

			//while New File x already exists, increase the x until it does not already exist.
			while(SimpleFileManager.FileExists("New File " + fileIndexCount.ToString()))
            {
				fileIndexCount++;
            }
			SimpleFileManager.ChangeFile("New File " + fileIndexCount.ToString(), emptyfiledata);
			fileIndexCount++;

			refreshFiles();
			saveCurrentFile();
			simpleModeFileListBox.Focus();
		}

		private void removeFileButton_Click(object sender, RoutedEventArgs e)
		{
			if (lastSelectedFileIndex != -1)
			{
				SimpleFileManager.DeleteFile(lastSelectedFileIndex);
				prevlbl = null;
			}
			refreshFiles();
		}

		private async void addRuleButton_Click(object sender, RoutedEventArgs e)
		{
			//add a rule to the ruleset file:
			//naam character: 0x00 + character voor variabele + 2 doubles + character voor action id + 2 keer 4 bytes voor args
			//voorbeeld: 0x01 (als variabele in deze range zit) + 0x01 (één v.d. vingers, xyz as van acceleratie of rotatie) + 0x03 (...) + arg1 t/m arg4
			//voorbeeld: 0x01 0x01 0x03 0x45 0x74 0x19 0x20

			//Moet een usercontrol invoegen (simplemodeparametereditor.xaml) aan de stackpanel in simplemode
			//moet naam geven denk ik

			//await FadePopup(saveErrorPopup);
			//List <simpleModeParameterEditor> parameterEditorList = new List<simpleModeParameterEditor>();

			 //loopen door de filedata en alles in parameter editor ui zetten
			if (lastSelectedFileIndex != -1)
			{
				//IList<FileData> information = SimpleFileManager.GetFileData(selectedIndex);
				//for(int i = 0; i < information.Count; i++)
				//            {
				//	simpleModeParameterEditor newEditor = new simpleModeParameterEditor();

				//            }
				parameterPanel.Items.Add(new simpleModeParameterEditor());
			}
		}

		void doNothing() { }
		private void removeRuleButton_Click(object sender, RoutedEventArgs e)
		{
			//verwijder geselecteerde parameterPanel
			if(parameterPanel.SelectedIndex != -1)
				parameterPanel.Items.RemoveAt(parameterPanel.SelectedIndex);

			//saveCurrentFile(); is not needed due to the program already saving the file when another file is clicked.
		}

		private void StartButton_Click(object sender, RoutedEventArgs e)
		{
			//Parse all the SMPE's into lua
			List<FileData> FD = new List<FileData>(SimpleFileManager.GetFileData(0));
			FD.Add(new FileData(0, 12, 26, 0, 4, 3));
			string parsed = LuaParser.Parse(FD.ConvertAll(x => x.toLogic()).ToList());

			//put the code into a file and run it
			parsed = SimpleFileManager.ChangeParsedFile(parsed);
			Communicator.load(parsed);
			Communicator.start();

			//start monitoring
			Startwindow sw = new Startwindow();
			sw.Show();
		}
		private void SimpleModeFileListBox_SelectionChanged(object sender, SelectionChangedEventHandler e)
		{
			saveCurrentFile();

		}
		#endregion

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#region ColourBrushes
		public static SolidColorBrush transparent = new SolidColorBrush(Color.FromArgb(0, 100, 100, 100));
		public static SolidColorBrush off_white = new SolidColorBrush(Color.FromArgb(255, 242, 242, 242));
		public static SolidColorBrush almostTransparentWhite = new SolidColorBrush(Color.FromArgb(80, 242, 242, 242));
		public static SolidColorBrush halfTransparentWhite = new SolidColorBrush(Color.FromArgb(140, 242, 242, 242));
		public static SolidColorBrush white = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
		public static SolidColorBrush black = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
		public static SolidColorBrush dark_blue = new SolidColorBrush(Color.FromArgb(255, 40, 120, 200));
		public static SolidColorBrush light_blue = new SolidColorBrush(Color.FromArgb(225, 110, 130, 255));
		public static SolidColorBrush vague_purple = new SolidColorBrush(Color.FromArgb(180, 160, 110, 255)); //nice purple
        #endregion


    }

}

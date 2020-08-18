using LeHandUI.Properties;
using Microsoft.TeamFoundation.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
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
using Color = System.Windows.Media.Color;
using Cursors = System.Windows.Input.Cursors;
using ListBox = System.Windows.Controls.ListBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using MouseEventHandler = System.Windows.Input.MouseEventHandler;
using TextBox = System.Windows.Controls.TextBox;

/*COLOUR SCHEME: #212121 , #065464 ,  #34acbc ,     #85c3cf ,      #7a7d84 
                   (33,33,33),   (6,84,100),(52,172,188), (133,195,207), (122,125,132)
                 very dark blue, dark blue, aqua, pale aqua,     greyish
*/

namespace LeHandUI
{
	public partial class SimpleMode : System.Windows.Controls.UserControl
	{
		public static string[] fileNames = SimpleFileManager.FileNames();
		public static List<TextBox> textBoxes = new List<TextBox>();

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

        #region initialisation
        public SimpleMode()
		{
			InitializeComponent();
		
			addFileImage.Source		=	ImageSourceFromBitmap(Properties.Resources.AddFile64x64);
			removeFileImage.Source	=	ImageSourceFromBitmap(Properties.Resources.RemoveFile64x64);
			addRuleImage.Source		=	ImageSourceFromBitmap(Properties.Resources.PALE_GREEN_AddIcon64x64);
			removeRuleImage.Source	=	ImageSourceFromBitmap(Properties.Resources.WASHED_OUT_RED_DeleteIcon64x64);

			simpleModeFileListBox.ItemsSource = textBoxes;

            /* //DATA FOR TEST FILES
			FileData[] data = new FileData[6];
			FileData[] newdata = new FileData[1];
			newdata[0] = new FileData(0, 0.2, 0.8, 0, 30, 0);
			SimpleFileManager.ChangeFile("kiekoek", newdata);
			SimpleFileManager.ChangeFile("halloe", newdata);*/

            //INITIALIZE THE TEXTBOXARRAY
            for (int i = 0; i < fileNames.Length; i++)
            {
				TextBox txtbox = new TextBox();
				StyleNonSelectedTextBox(txtbox);
				txtbox.Text = fileNames[i];

				textBoxes.Add(txtbox);
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

			textBoxes.Clear(); //NOW DISABLED TO IMPROVE EFFICIENCY BY ALTERING REFRESH FILE MECHANISM, ENABLE IF BUGGY (voor douwe ursingur)

			//simpleModeFileListBox.Items.Clear();
			fileNames = SimpleFileManager.FileNames();
			for (int i = 0; i < fileNames.Length; i++)
			{

				TextBox txtbox = new TextBox();
				txtbox.Text = fileNames[i];

				StyleNonSelectedTextBox(txtbox);

				textBoxes.Add(txtbox);

			}
			simpleModeFileListBox.Items.Refresh();
		}


		#region StyleFunctions

		public void StyleNonSelectedTextBox(TextBox txtbox)
        {
			txtbox.IsReadOnly = true;
			txtbox.Focusable = true;

			//txtbox.Cursor = Cursors.Arrow;

			txtbox.Background = transparent;
			txtbox.Foreground = off_white;
			txtbox.BorderThickness = new Thickness(0);
			txtbox.BorderBrush = transparent;

			txtbox.KeyDown += Txtbox_KeyDown;
			txtbox.LostKeyboardFocus += Txtbox_LostKeyboardFocus;
			txtbox.MouseDoubleClick += Txtbox_MouseDoubleClick;
			txtbox.MouseDown += onListItemSelected;
            txtbox.MouseEnter += TxtBox_MouseEnter;
			txtbox.MouseLeave += onListItemLeave;
			txtbox.TextChanged += onTextChanged;
		}

		public void styleMouseOverTextBox(TextBox txtbox)
        {
			txtbox.Cursor = Cursors.Hand;

			txtbox.Background = almostTransparentWhite;
			txtbox.Foreground = white;

		}

		public void StyleSelectedTextBox(TextBox txtbox)
        {
			txtbox.IsReadOnly = true;

			txtbox.Background = halfTransparentWhite;
			txtbox.Foreground = white;
			txtbox.BorderThickness = new Thickness(0);
			txtbox.BorderBrush = white;

			txtbox.Cursor = Cursors.IBeam;
		}

		#endregion


		public void saveRuleFromCurrentFile()
		{
			//get all rules from current file
			int selectedIndex = simpleModeFileListBox.SelectedIndex;
			if (selectedIndex != -1)
			{
				IList<FileData> currentFileData = SimpleFileManager.GetFileData(selectedIndex);
			}

			//fix the rest lazy piece of shit douwe

		}

		
        #region Txtbox Handlers
        private void Txtbox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				lostfocus(sender);
			}
		}
		private void Txtbox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			lostfocus(sender);
			
		}

		private void TxtBox_MouseEnter(object sender, MouseEventArgs e)
        {
			TextBox txtbox = (TextBox)sender;

        }

		private void Txtbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

			TextBox txtbox = (TextBox)sender;
			prevname = txtbox.Text;

			StyleSelectedTextBox(txtbox);
		}

		private void lostfocus(object sender) //if the text is altered in the textbox, replace the name in the filemanager with the textbox
		{
			TextBox txtbox = (TextBox)sender;

			if (txtbox != simpleModeFileListBox.SelectedItem) //werkt dit? geen flauw idee, vast wel
			{
				StyleNonSelectedTextBox(txtbox);
			}
            else
            {
				
            }

			int index = simpleModeFileListBox.Items.IndexOf(sender);
			if (txtbox.Text.Length > 1)
			{
				SimpleFileManager.ChangeName(prevname, txtbox.Text);
				refreshFiles();
			}
			else
			{
				txtbox.Text = prevname;
			}
		}
        #endregion

        #region ListBox Handlers
        private void onTextChanged(object sender, TextChangedEventArgs e)
		{
			/*TextBox listitem = (TextBox)sender;
			int index = simpleModeFileListBox.SelectedIndex;
			if (index != -1)
			{
				SimpleFileManager.ChangeName(fileNames[index], listitem.Text);
				refreshFiles();
				dosumShit(sender);
			}*/
		}
		private void onListItemSelected(object sender, System.Windows.Input.MouseEventArgs e)
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
		}
		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			saveRuleFromCurrentFile();
		}
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
			saveRuleFromCurrentFile();
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

			int selectedIndex = simpleModeFileListBox.SelectedIndex;

			saveRuleFromCurrentFile();
		}
		private void removeRuleButton_Click(object sender, RoutedEventArgs e)
		{
			//remove the shit above

			saveRuleFromCurrentFile();
		}
		#endregion

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		
	}
}

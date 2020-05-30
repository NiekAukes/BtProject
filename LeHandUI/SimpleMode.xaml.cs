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
		
		SolidColorBrush transparent =	new SolidColorBrush(Color.FromArgb(0, 100, 100, 100));
		SolidColorBrush off_white	=	new SolidColorBrush(Color.FromArgb(255,242,242,242));
		SolidColorBrush white		=	new SolidColorBrush(Color.FromArgb(255,255,255,255));
		SolidColorBrush black =			new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
		SolidColorBrush dark_blue	=	new SolidColorBrush(Color.FromArgb(178, 6, 84, 100));

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

		public SimpleMode()
		{
			InitializeComponent();
		
			addFileImage.Source		=	ImageSourceFromBitmap(Properties.Resources.AddFile64x64);
			removeFileImage.Source	=	ImageSourceFromBitmap(Properties.Resources.RemoveFile64x64);
			addRuleImage.Source		=	ImageSourceFromBitmap(Properties.Resources.PALE_GREEN_AddIcon64x64);
			removeRuleImage.Source	=	ImageSourceFromBitmap(Properties.Resources.WASHED_OUT_RED_DeleteIcon64x64);

			simpleModeFileListBox.ItemsSource = textBoxes;

			FileData[] data = new FileData[6];
			FileData[] newdata = new FileData[1];
			newdata[0] = new FileData(0, 0.2, 0.8, 0, 30, 0);
			SimpleFileManager.ChangeFile("kiekoek", newdata);
			SimpleFileManager.ChangeFile("halloe", newdata);

			refreshFiles();

			//getfiledata returns null, pls fix volvo
			FileData dat = SimpleFileManager.GetFileData(0)[0];
			
		}

		private string prevname = "something wrong";
		public void refreshFiles()
		{
			fileNames = null;
			textBoxes.Clear();
			//simpleModeFileListBox.Items.Clear();
			fileNames = SimpleFileManager.FileNames();
			for (int i = 0; i < fileNames.Length; i++)
			{

				var txtbox = new TextBox();
				txtbox.IsReadOnly = true;
				txtbox.Text = fileNames[i];
				txtbox.Focusable = true;

				txtbox.KeyDown += Txtbox_KeyDown;
				txtbox.LostKeyboardFocus += Txtbox_LostKeyboardFocus;
				txtbox.MouseDoubleClick += Txtbox_MouseDoubleClick;
				txtbox.MouseDown += onListItemSelected;
				txtbox.MouseLeave += onListItemLeave;
				txtbox.TextChanged += onTextChanged;

				txtbox.Cursor = Cursors.Arrow;

				txtbox.Background = transparent;
				txtbox.Foreground = off_white;
				txtbox.BorderThickness = new Thickness(2);
				txtbox.BorderBrush = transparent;
				textBoxes.Add(txtbox);
				
			}
			simpleModeFileListBox.Items.Refresh();
		}

		private void Txtbox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				lostfocus(sender);
			}
		}
		private void lostfocus(object sender)
		{
			TextBox txtbox = (TextBox)sender;
			txtbox.IsReadOnly = true;

			TextBox listitem = (TextBox)sender;
			listitem.Background = transparent;
			listitem.Foreground = off_white;
			listitem.BorderThickness = new Thickness(2);
			listitem.BorderBrush = transparent;

			int index = simpleModeFileListBox.Items.IndexOf(sender);
			if (listitem.Text.Length > 1)
			{
				SimpleFileManager.ChangeName(prevname, listitem.Text);
				refreshFiles();
			}
			else
			{
				listitem.Text = prevname;
			}
		}
		private void Txtbox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			lostfocus(sender);
			
		}

		private void Txtbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

			TextBox txtbox = (TextBox)sender;
			txtbox.IsReadOnly = false;
			prevname = txtbox.Text;

			TextBox listitem = (TextBox)sender;
			listitem.Background = white;
			listitem.Foreground = black;
			listitem.BorderThickness = new Thickness(2);
			listitem.BorderBrush = white;

			listitem.Cursor = Cursors.IBeam;
		}

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

		private void ApplyNameListBoxItem(object sender, RoutedEventArgs e)
		{
			ItemCollection boxItemsshit = simpleModeFileListBox.Items;
			for (int index = 0; index < fileNames.Length; index++)
			{
				//splits the given name (something like something.something.TextBox: name) and gets the actual name
				string updatedName = (boxItemsshit[index].ToString()).Split(':')[1]; 

				SimpleFileManager.ChangeName(fileNames[index], updatedName);
			}
			refreshFiles();
			dosumShit(sender);
			
		}

		private void onListItemSelected(object sender, System.Windows.Input.MouseEventArgs e)
		{
			UIElement UIE = (UIElement)sender;
			UIE.Focus();
			TextBox listitem = (TextBox)sender;
			listitem.Background			= dark_blue;
			listitem.Foreground			= white;
			listitem.BorderThickness	= new Thickness(2);
			listitem.BorderBrush		= white;

		}
		private void onListItemLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			//dosumShit(sender);
		}
		private void dosumShit(object sender)
		{
			TextBox listitem = (TextBox)sender;
			MainWindow.inst.Focus();
			listitem.Background = transparent;
			listitem.Foreground = off_white;
			listitem.BorderThickness = new Thickness(2);
			listitem.BorderBrush = transparent;
		}

		private void addFileButton_Click(object sender, RoutedEventArgs e)
		{
			//add a filename + registry key for ruleset
			
			
		}
		private void removeFileButton_Click(object sender, RoutedEventArgs e)
		{
			//remove the shit above

		}
		private void addRuleButton_Click(object sender, RoutedEventArgs e)
		{
			//add a rule to the ruleset file:
			//naam character: 0x00 + character voor variabele + 2 doubles + character voor action id + 2 keer 4 bytes voor args
			//voorbeeld: 0x01 (als variabele in deze range zit) + 0x01 (één v.d. vingers, xyz as van acceleratie of rotatie) + 0x03 (...) + arg1 t/m arg4
			//voorbeeld: 0x01 0x01 0x03 0x45 0x74 0x19 0x20

		}
		private void removeRuleButton_Click(object sender, RoutedEventArgs e)
		{
			//remove the shit above

		}
		private void simpleModeFileListBox_doubleClick(object sender, EventArgs e)
		{
			int selectedIndex = simpleModeFileListBox.SelectedIndex;
			if (selectedIndex != -1)
			{
				simpleModeFileListBox_editName(selectedIndex);
			}
		}
		private void simpleModeFileListBox_editName(int index)
		{

		}
		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	}
}

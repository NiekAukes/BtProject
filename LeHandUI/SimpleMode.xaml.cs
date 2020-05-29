using Microsoft.TeamFoundation.Server;
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

namespace LeHandUI
{
	public partial class SimpleMode : System.Windows.Controls.UserControl
	{
		private string _popuptxt = "New name of file";
		public string popupText { get { return _popuptxt; } set { if (value != _popuptxt) { _popuptxt = value; OnPropertyChanged("popupText"); } } }


		public static string[] fileNames = SimpleFileManager.FileNames();
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

			addFileImage.Source = ImageSourceFromBitmap(Properties.Resources.AddFile64x64);
			removeFileImage.Source = ImageSourceFromBitmap(Properties.Resources.RemoveFile64x64);
			addRuleImage.Source = ImageSourceFromBitmap(Properties.Resources.PALE_GREEN_AddIcon64x64);
			removeRuleImage.Source = ImageSourceFromBitmap(Properties.Resources.WASHED_OUT_RED_DeleteIcon64x64);

			popupText = "Type your new name here";
			FileData[] data = new FileData[6];

			SimpleFileManager.ChangeFile("SomeFile", data);
			refreshFiles();

			SimpleFileManager.ChangeFile("BigDingdongdikkeBingBong", data);
			refreshFiles();
		}


		private void simpleModeFileListBox_doubleClick(object sender, EventArgs e)
		{
			int selectedIndex = simpleModeFileListBox.SelectedIndex;
			if(selectedIndex != -1)
			{
				simpleModeFileListBox_editName(selectedIndex);
			}
		}
		private void simpleModeFileListBox_editName(int index)
		{

		}
		public void refreshFiles()
		{
			fileNames = null;
			simpleModeFileListBox.Items.Clear();
			fileNames = SimpleFileManager.FileNames();
			for (int i = 0; i < fileNames.Length; i++)
			{
				simpleModeFileListBox.Items.Add(fileNames[i]);
			}
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
		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

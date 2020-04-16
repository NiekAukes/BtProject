using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;

using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Search;
using Microsoft.Win32;



namespace LeHandUI
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		//Dit is mijn mooie gekopieerde stackoverflow code
		//If you get 'dllimport unknown'-, then add 'using System.Runtime.InteropServices;'
		[DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject([In] IntPtr hObject);

		public ImageSource ImageSourceFromBitmap(Bitmap bmp)
		{
			var handle = bmp.GetHbitmap();
			try
			{
				return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			}
			finally { DeleteObject(handle); }
		}

		public MainWindow()
		{
			//Communicator.Init();
			InitializeComponent();

			//Alle icoontjes
			PlusIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.AddIcon16x16);
			DeleteIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.DeleteIcon16x16);
			RefreshIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.RefreshIcon16x16);
		}
		//Handlers for custom titlebar buttons
		private void MinimizeWindow(object sender, EventArgs e){
			App.Current.MainWindow.WindowState = WindowState.Minimized;
		}
		private void MaximizeWindow(object sender, EventArgs e){
			if (App.Current.MainWindow.WindowState == WindowState.Maximized)
			{
				App.Current.MainWindow.WindowState = WindowState.Normal;
			}
			else if (App.Current.MainWindow.WindowState == WindowState.Normal)
			{
				App.Current.MainWindow.WindowState = WindowState.Maximized;
			}
		}
		private void CloseWindow(object sender, EventArgs e){
			App.Current.MainWindow.Close();
		}
		private void DragStart(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
		}
	}
}
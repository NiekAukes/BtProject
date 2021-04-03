using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brush = System.Windows.Media.Brush;

namespace LeHandUI
{
	public partial class MainWindow : Window
	{
		public static string Directory = (string)Registry.CurrentUser.OpenSubKey("Software\\LeHand").GetValue("Dir");

		public static MainWindow inst = null;
		public static System.Windows.Controls.ListBox Listbox = null;

		AdvancedMode advancedModeChild = new AdvancedMode();
		SimpleMode simpleModeChild = new SimpleMode();
		public static Brush greyedOutColour = new SolidColorBrush(System.Windows.Media.Color.FromArgb(30, 40, 40, 40));
		public static Brush FocusedColour = new SolidColorBrush(System.Windows.Media.Color.FromArgb(30,242,242,242));

		public static Bluetooth BTService = new Bluetooth();

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

        
		void OnWindowLoaded(object sender, EventArgs e)
		{
			Communicator.Init();
			BTService.RefreshDevices();
		}

		public MainWindow()
		{
			Loaded += OnWindowLoaded;
			inst = this;
			InitializeComponent();
			FileManager.LoadAllFiles();
			//Bluetooth bt = new Bluetooth();
			//Logic[] logics =
			//	{
			//		new Logic(1, 0.2, 0.4, new Kpress('k')),
			//		new Logic(3, 0.6, 0.8, new Kpress('l')),
			//		new Logic(5, 0.152, 0.534, new Kpress('m')),
			//	};
			//string s = LuaParser.Parse(logics);
			//FileStream stream = File.OpenWrite("yeshere.lua");
			//StreamWriter writer = new StreamWriter(stream);
			//writer.Write(s);
			//writer.Close();
			//stream.Close();

			settingsImage.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.Settings64x64);
		}
		public bool settingsIsClosed = false;
		
		private void settingsButton_Load(object sender, RoutedEventArgs e)
        {
			//Load settings panel
			if (SettingsWindow.inst == null)
			{
				SettingsWindow.inst = new SettingsWindow();
				SettingsWindow.inst.Show();
			}
			else
			{
				if (!SettingsWindow.inst.IsActive && settingsIsClosed)
				{
					SettingsWindow.inst = new SettingsWindow();
					SettingsWindow.inst.Show();
					SettingsWindow.inst.WindowState = WindowState.Normal;
					SettingsWindow.inst.Focus();
					settingsIsClosed = false;
				}
				else
				{
					SettingsWindow.inst.Close();
					settingsIsClosed = true;
				}
			}
        }

		#region WindowsButtonHandlers, no touchy pls
		private void MinimizeWindow(object sender, EventArgs e){
			for(int i = 0; i < App.Current.Windows.Count; i++)
            {
				App.Current.Windows[i].WindowState = WindowState.Minimized;
            }
			//App.Current.MainWindow.WindowState = WindowState.Minimized;
		}
		private void MaximizeWindow(object sender, EventArgs e){
			//App.Current.MainWindow.Focus();

			if (MainWindow.inst.WindowState == WindowState.Maximized)
			{
				restoreButonPath.Data = Geometry.Parse("M 18.5,10.5 H 27.5 V 19.5 H 18.5 Z");


				MainWindow.inst.WindowState = WindowState.Normal;
			}
			else if (MainWindow.inst.WindowState == WindowState.Normal)
			{
				restoreButonPath.Data = Geometry.Parse("M 18.5,12.5 H 25.5 V 19.5 H 18.5 Z M 20.5,12.5 V 10.5 H 27.5 V 17.5 H 25.5");
				MainWindow.inst.WindowState = WindowState.Maximized;
			}
		}
		private void CloseWindow(object sender, EventArgs e){
			if (new List<bool>(FileManager.isFileNotSaved).Contains(true))
			{
				var res = MessageBox.Show("There are unsaved files, do you want to save all?", "Unsaved Files", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation);

				if (res == MessageBoxResult.Yes)
				{
					//save all files first
					FileManager.SaveAll();
				}
				else if (res == MessageBoxResult.Cancel)
				{
					//cancel closing
					return;
				}
			}
			Application.Current.Shutdown();
		}
		private void DragStart(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
		}
        #endregion

        #region MenuFunctionality_button_clicks
		public void addElementToPanelAndRemoveOtherElement(DockPanel panel, UIElement element, UIElement elementToRemove)
		{
			if (!panel.Children.Contains(element))
			{
				panel.Children.Add(element);
			}
			if (panel.Children.Contains(elementToRemove))
			{
				panel.Children.Remove(elementToRemove);
			}
		}

		//It just hides and makes other usercontrols visible, I don't want to fuck with adding child elements to the dockpanel, can't be bothered to be honest
        private void SimpleButton_Load(object sender, RoutedEventArgs e)
		{
			addElementToPanelAndRemoveOtherElement(ViewSwitcher, simpleModeChild, advancedModeChild);

		}

		private void AdvancedButton_Load(object sender, RoutedEventArgs e)
		{
			addElementToPanelAndRemoveOtherElement(ViewSwitcher, advancedModeChild,simpleModeChild);

		}

        private void mainWindowName_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
			

		}
        #endregion


    }
}
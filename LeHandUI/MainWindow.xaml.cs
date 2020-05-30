using Microsoft.Win32;
using System;
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

        /*Stopwatch refreshTimer = new Stopwatch();
		int SelectedItemIndex;
		bool hasRefreshOccurredWithinSeconds = false;*/

		/*
		private void LoadLuaFileFromSelectedObjectInList(object sender, EventArgs e) {
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
		
		private void RemoveLuaScript(object sender, EventArgs e) {
			int idToBeRemoved = -1; //some ridiculous number, i. e. -1 just isn't possible

			if (LuaFileView.SelectedIndex != -1) {

				idToBeRemoved = (LuaFileView.SelectedIndex);
				int[] allIds = LHregistry.GetAllFileIds();

				if (allIds.Length > idToBeRemoved && idToBeRemoved != -1) {
					LHregistry.RemoveFile(allIds[idToBeRemoved]);



					LuaFileView.Items.RemoveAt(idToBeRemoved);
				}
			}

			//ALWAYS REFRESH, saves some headaches, like trying to solve a nonexistent problem for two hours. Trust me, I know.
			LuaFileView.Items.Refresh();
			
		}
		
		private void RefreshLuaScript(object sender, EventArgs e) {
			
			if (hasRefreshOccurredWithinSeconds == false) { //if the refresh has not occurred in x milliseconds
				List<string> LuaNames = new List<string>(LHregistry.GetAllFilenames());
				for (int i = 0; i < LuaNames.Count; i++)
				{
					LuaNames.Add(LHregistry.getSimpleName(LuaNames[i]));
					LuaFileView.Items.Add(LuaNames[i]);
				}
				
			}
			else{}

			hasRefreshOccurredWithinSeconds = true;
		}
		public void ChangeTextBoxText(string label, int index){
			LuaFileView.Items.RemoveAt(index);
			LuaFileView.Items.Insert(index, label);
		}
		public static void ChangeTextBoxText(System.Windows.Controls.ListBox list, string label, int index){
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
					ChangeTextBoxText(list, label, index);
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
						ChangeTextBoxText(label, index);
					}
				}
			}
			BypassTextChangedEvent = false;
		}
		private void AddReferenceScript(object sender, EventArgs e) {
			Microsoft.Win32.OpenFileDialog openFileExplorer = new Microsoft.Win32.OpenFileDialog(){
				CheckFileExists = true,
				CheckPathExists = true,
				InitialDirectory = @"Documents",
				ShowReadOnly = true,
				Filter = " All files(*.*) 'cuz .lua doesn't work|*.*"
			};

			Nullable<bool> result = openFileExplorer.ShowDialog();
			if (result == true){
				string newFilePath = openFileExplorer.FileName;
				int newFileId = FileManager.AddReference(newFilePath);
				LuaFileView.Items.Add(LHregistry.getSimpleName(newFilePath));
				
			}

		}
		private void SaveScript(object sender, EventArgs e)
		{
			string writePath = LHregistry.GetFile(FileManager.currentFile);
			try {
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
		*/
		void OnWindowLoaded(object sender, EventArgs e)
		{
			Communicator.Init();
		}
		private void ChangeBackground(System.Windows.Controls.Button buttontochange, int a, int r, int g, int b)
		{
			buttontochange.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(a), (byte)(r), (byte)(g), (byte)(b)));
		}

		public MainWindow()
		{
			Loaded += OnWindowLoaded;
			inst = this;
			InitializeComponent();
			FileManager.LoadAllFiles();
			
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

			ProgramIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.BTIconNew);
		}


		#region TitleBarButtonHandlers
		private void MinimizeWindow(object sender, EventArgs e){
			App.Current.MainWindow.WindowState = WindowState.Minimized;
		}
		private void MaximizeWindow(object sender, EventArgs e){
			if (App.Current.MainWindow.WindowState == WindowState.Maximized)
			{
				restoreButonPath.Data = Geometry.Parse("M 18.5,10.5 H 27.5 V 19.5 H 18.5 Z");


				App.Current.MainWindow.WindowState = WindowState.Normal;
			}
			else if (App.Current.MainWindow.WindowState == WindowState.Normal)
			{
				restoreButonPath.Data = Geometry.Parse("M 18.5,12.5 H 25.5 V 19.5 H 18.5 Z M 20.5,12.5 V 10.5 H 27.5 V 17.5 H 25.5");
				App.Current.MainWindow.WindowState = WindowState.Maximized;
			}
		}
		private void CloseWindow(object sender, EventArgs e){
			Communicator.quit();
			App.LeHandExited = true;
			this.Close();
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
		#endregion
	}
}
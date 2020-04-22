using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
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

		//Niek heeft al mooi een functie gemaakt die een string[] returnt met alle paths
		//DRIE FUNCTIES: ADD / REFERENCE FILE, REMOVE FILE, REFRESH FILES
		List<string> LuaNames = new List<string>();

		Stopwatch refreshTimer = new Stopwatch();
		int SelectedItemIndex;
		long elapsedTime;
		bool hasRefreshOccurredWithinSeconds = false;
		int currentSelectedId = -1;

		private void LoadLuaFileFromSelectedObjectInList(object sender, EventArgs e) {
			ListBox naam = (ListBox)(sender);
			SelectedItemIndex = naam.SelectedIndex;
			int[] id = LHregistry.GetAllFileIds();
			int ActualFileId = id[SelectedItemIndex];

			string FileContents = FileManager.LoadFile(ActualFileId);
			textEditor.Text = FileContents;
		}

		/*private void AddLuaScript(object sender, EventArgs e) {
			string filePath = "HKEY_CURRENT_USER\\Software\\LeHand\\Filenames\\Testfile1.lua";
			int newFileId = FileManager.Addfile(filePath);
			LuaNames.Add(LHregistry.getSimpleName(filePath));
			LuaFileView.Items.Refresh();
		}*/
		private void RemoveLuaScript(object sender, EventArgs e) {
			int idToBeRemoved = -1; //some ridiculous number
			if (LuaFileView.SelectedIndex != -1) {
				idToBeRemoved = (LuaFileView.SelectedIndex);
				int[] allIds = LHregistry.GetAllFileIds();
				if (allIds.Length > idToBeRemoved && idToBeRemoved != -1) {
					LHregistry.RemoveFile(allIds[idToBeRemoved]);
					LuaNames = new List<string>(LHregistry.GetAllFilenames());
					//LuaFileView.Items.Remove(LuaFileView.SelectedItem.ToString());
					LuaFileView.UpdateLayout();
				}
				else { }
			}

			//ALWAYS REFRESH, saves some headaches, like trying to solve a nonexistent problem for two hours. Trust me, I know.
			LuaFileView.Items.Refresh();
			
		}
		private void RefreshLuaScript(object sender, EventArgs e) {
			
			if (hasRefreshOccurredWithinSeconds == false) { //if the refresh has not occurred in x milliseconds
				LuaNames = null;
				LuaNames = new List<string>(LHregistry.GetAllFilenames());
				int currentLuaNamesCount = LuaNames.Count;
				for (int i = 0; i < currentLuaNamesCount; i++)
				{
					LuaNames.Add(LHregistry.getSimpleName(LuaNames[i]));
				}
				
			}
			else{}
			hasRefreshOccurredWithinSeconds = true;

			LuaFileView.Items.Refresh();
		}
		private void AddReferenceScript(object sender, EventArgs e) {
			OpenFileDialog openFileExplorer = new OpenFileDialog()
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
				LuaNames.Add(LHregistry.getSimpleName(newFilePath));
				
			}

			LuaFileView.Items.Refresh();
		}

		public MainWindow()
		{
			//Communicator.Init();
			InitializeComponent();

			FileManager.LoadAllFiles();
			

			LuaNames = new List<string>(LHregistry.GetAllFilenames());

			for(int i = 0; i < LuaNames.Count; i++)
			{
				LuaNames[i] = LHregistry.getSimpleName(LuaNames[i]);
			}

			//Alle icoontjes
			PlusIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.AddIcon16x16);
			DeleteIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.DeleteIcon16x16);
			RefreshIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.RefreshIcon16x16);
			//AddReferenceIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.AddReference16x16);


			ProgramIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.BTIconNew);

			LuaFileView.ItemsSource = LuaNames;

			

			
			/*while (!false)
			{
				if (hasRefreshOccurredWithinSeconds)
				{
					refreshTimer.Start();
				}
				else if(refreshTimer.ElapsedMilliseconds > 2000)
				{
					hasRefreshOccurredWithinSeconds = false;
					refreshTimer.Stop();
					refreshTimer.Reset();
				}
				else { }
			}*/
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
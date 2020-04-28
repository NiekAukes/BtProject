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
		//List<string> LuaNames = new List<string>();

		Stopwatch refreshTimer = new Stopwatch();
		int SelectedItemIndex;
		bool hasRefreshOccurredWithinSeconds = false;
		int currentSelectedId = -1;

		private void LoadLuaFileFromSelectedObjectInList(object sender, EventArgs e) {
			ListBox naam = (ListBox)(sender);
			SelectedItemIndex = naam.SelectedIndex;
			int[] id = LHregistry.GetAllFileIds();
			int ActualFileId = id[SelectedItemIndex];

			string FileContents = FileManager.LoadFile(ActualFileId);
			if (FileContents != null)
				textEditor.Text = FileContents;
		}
		/*private void AddLuaScript(object sender, EventArgs e) {
			string filePath = "HKEY_CURRENT_USER\\Software\\LeHand\\Filenames\\Testfile1.lua";
			int newFileId = FileManager.Addfile(filePath);
			LuaNames.Add(LHregistry.getSimpleName(filePath));
			LuaFileView.Items.Refresh();
		}*/
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
				LuaFileView.Items.Add(LHregistry.getSimpleName(newFilePath));
				
			}

		}
		private void SaveScript(object sender, EventArgs e)
		{
			return;
		}
		private void RunLuaScript(object sender, EventArgs e)
		{
			return;
		}


		public MainWindow()
		{
			//Communicator.Init();
			InitializeComponent();

			FileManager.LoadAllFiles();
			

			List<string> LuaNames = new List<string>(LHregistry.GetAllFilenames());

			for(int i = 0; i < LuaNames.Count; i++)
			{
				LuaNames[i] = LHregistry.getSimpleName(LuaNames[i]);
				LuaFileView.Items.Add(LuaNames[i]);
			}

			//Alle icoontjes
			PlusIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.PALE_GREEN_AddIcon64x64);
			DeleteIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.WASHED_OUT_RED_DeleteIcon64x64);
			RefreshIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.AQUA_RefreshIcon64x64);
			//AddReferenceIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.AddReference16x16);
			SaveIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.SaveScript64x64);
			RunPrgmIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.StartScript64x64);

			ProgramIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.BTIconNew);

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
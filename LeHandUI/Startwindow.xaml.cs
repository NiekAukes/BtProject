using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LeHandUI
{
    public partial class Startwindow : Window
    {
        public Startwindow()
        {
            InitializeComponent();
     
        }







        //Handlers for custom titlebar buttons
        #region TitleBarButtonHandlers
        private void MinimizeWindow(object sender, EventArgs e)
		{
			App.Current.MainWindow.WindowState = WindowState.Minimized;
		}
		private void MaximizeWindow(object sender, EventArgs e)
		{
			if (App.Current.MainWindow.WindowState == WindowState.Maximized)
			{
				App.Current.MainWindow.WindowState = WindowState.Normal;
			}
			else if (App.Current.MainWindow.WindowState == WindowState.Normal)
			{
				App.Current.MainWindow.WindowState = WindowState.Maximized;
			}
		}
		private void CloseWindow(object sender, EventArgs e)
		{
			Close();
		}
		private void DragStart(object sender, MouseButtonEventArgs e)
		{
			this.DragMove();
		}
		#endregion
	}
}

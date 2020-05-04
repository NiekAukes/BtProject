using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;

namespace LeHandUI
{
    public partial class Startwindow : Window
    {
        #region ImageSourceFromBitmap_func
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
        #endregion

        public Startwindow()
        {
            InitializeComponent();
            ProgramIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.BTIconNew);

            //assigning values to all chart values in chart
            ChartValues<double> XvalueList = new ChartValues<double>();
            double[] newXVals = { 2, 5, 6, 6 };
            addChartValues(newXVals, XvalueList);

            ChartValues<double> YvalueList = new ChartValues<double>();
            double[] newYVals = { 0,2,8,4 };
            addChartValues(newYVals, YvalueList);
            
            ChartValues<double> ZvalueList = new ChartValues<double>();
            double[] newZVals = { 5,3,5,4 };
            addChartValues(newZVals, ZvalueList);

            cartesianchart1.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "X",
                    Values =  XvalueList,  //new ChartValues<double> {4, 6, 5, 2, 7}
                    PointGeometry = DefaultGeometries.Diamond,
                    PointGeometrySize = 4,
                    LineSmoothness = 1,
                    StrokeThickness = 2,
                    Stroke = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0x00, 0xFF,0xFF,0x90))
                },
                new LineSeries
                {
                    Title = "Y",
                    Values = YvalueList,
                    PointGeometry = DefaultGeometries.Cross,
                    PointGeometrySize = 4,
                    LineSmoothness = 1
                },
                new LineSeries
                {
                    Title = "Z",
                    Values = ZvalueList,
                    PointGeometry = DefaultGeometries.Square,
                    PointGeometrySize = 4,
                    LineSmoothness = 1
                }
            };

            cartesianchart1.AxisX.Add(new Axis
            {
                Title = "Time",
                Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" }
            });

            cartesianchart1.AxisY.Add(new Axis
            {
                Title = "Value",
                LabelFormatter = value => value.ToString("C")
            });

            
            cartesianchart1.DataClick += CartesianChart1OnDataClick;





        }
        #region addChartValues_func
        public void addChartValues(double[] valuelist, ChartValues<double> chartlist)
        {
            for (int i=0;i<valuelist.Length;i++)
            {
                chartlist.Add(valuelist[i]);
            }
        }
        #endregion
        private void CartesianChart1OnDataClick(object sender, ChartPoint chartPoint)
        {
            return;
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

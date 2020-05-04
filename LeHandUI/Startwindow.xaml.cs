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

/*COLOUR SCHEME: #212121 , #065464 ,  #34acbc ,     #85c3cf ,      #7a7d84 
                   (33,33,33),   (6,84,100),(52,172,188), (133,195,207), (122,125,132)
                 very dark blue, dark blue,  aqua,           pale aqua,     greyish */

namespace LeHandUI
{
    public partial class Startwindow : Window
    {
        int[] graphs = { };
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

        public static Startwindow inst = null;

        static ChartValues<double> AccXList         =   new ChartValues<double>();
        static ChartValues<double> AccYList         =   new ChartValues<double>();
        static ChartValues<double> AccZList         =   new ChartValues<double>();

        static ChartValues<double> RotXList         =   new ChartValues<double>();
        static ChartValues<double> RotYList         =   new ChartValues<double>();
        static ChartValues<double> RotZList         =   new ChartValues<double>();

        static ChartValues<double> DuimList         =   new ChartValues<double>();
        static ChartValues<double> WijsvingerList   =   new ChartValues<double>();
        static ChartValues<double> MiddelvingerList =   new ChartValues<double>();
        static ChartValues<double> RingvingerList   =   new ChartValues<double>();
        static ChartValues<double> PinkList         =   new ChartValues<double>();


        //IMPORTANT GODVER
        //ACCGRAPHVALUES is master object, die de graph values houdt van de lijsten
        public static ChartValues<double>[] AllGraphValues = { AccXList, AccYList, AccZList, 
                                                        RotXList, RotYList, RotZList,
                                                        DuimList, WijsvingerList, MiddelvingerList, RingvingerList, PinkList};
        // [0-2]  Acceleration
        // [3-5]  Rotation
        // [6-10] Fingers

        public Startwindow()
        {
            inst = this;
            InitializeComponent();
            ProgramIcon.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.BTIconNew);

            //REDUNDANT LISTS FOR TEST VALUES, TO BE REMOVED IN NEXT IMPLEMENTATION
            AccXList.AddRange(new ChartValues<double>(new double[] { 2, 5, 6, 6 }));
            AccYList.AddRange(new ChartValues<double>(new double[] { 0, 2, 8, 4 }));
            AccZList.AddRange(new ChartValues<double>(new double[] { 5, 3, 5, 4 }));

            RotXList.AddRange(new ChartValues<double>(new double[] { 0, 0, 2, 4 }));
            RotYList.AddRange(new ChartValues<double>(new double[] { 1, 2, 7, 3 }));
            RotZList.AddRange(new ChartValues<double>(new double[] { 3, 0, 0, 0 }));

            DuimList.AddRange(new ChartValues<double>(new double[] { 5, 4, 7, 5 }));
            WijsvingerList.AddRange(new ChartValues<double>(new double[] { 0,6,8,5 }));
            MiddelvingerList.AddRange(new ChartValues<double>(new double[] { 3,8,5,6 }));
            RingvingerList.AddRange(new ChartValues<double>(new double[] { 8,6,7,5 }));
            PinkList.AddRange(new ChartValues<double>(new double[] { 5,7,4,6 }));

            AccelerationGraph.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "X",
                    Values =  AccXList,  //new ChartValues<double> {4, 6, 5, 2, 7}
                    PointGeometry = DefaultGeometries.Diamond,
                    PointGeometrySize = 4,
                    LineSmoothness = 1,
                    StrokeThickness = 4,
                    Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(52,172,188)),
                    Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0,52,172,188)) //Completely transparent but if you want you can enable it
                },
                new LineSeries
                {
                    Title = "Y",
                    Values = AccYList,
                    PointGeometry = DefaultGeometries.Circle,
                    PointGeometrySize = 4,
                    LineSmoothness = 1,
                    StrokeThickness = 4,
                    Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255,19,120)), //#E11378
                    Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0,255,19,120)) // same here, just insert 120 into the alpha channel
                },
                new LineSeries
                {
                    Title = "Z",
                    Values = AccZList,
                    PointGeometry = DefaultGeometries.Square,
                    PointGeometrySize = 4,
                    LineSmoothness = 1,
                    StrokeThickness = 4,
                    Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255,217,89)), //#FFD959
                    Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0,255,217,89))
                }
            };
            AccelerationGraph.AxisX.Add(new Axis
            {
                Title = "Refreshes",
                Labels = null
            });
            AccelerationGraph.AxisY.Add(new Axis
            {
                Title = "Value",
                LabelFormatter = null
            });

            AccelerationGraph.DisableAnimations = true;
            AccelerationGraph.DataClick += CartesianChart1OnDataClick;





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

        public static void addNodeToGraph(int graphId, double value)
        {
            //Automatic Checker if value of a list has gone past 20, assuming an update rate of 200ms per update means last 4 seconds stays in graph
            for (int i = 0; i < AllGraphValues.Length; i++){
                if(AllGraphValues[i].Count > 100)
                {
                    while(AllGraphValues[i].Count > 100)
                    {
                        AllGraphValues[i].RemoveAt(0); //removes first value of list
                    }
                }
            }
            AllGraphValues[graphId].Add(value);
            return;
        }

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

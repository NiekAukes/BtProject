using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections.ObjectModel;
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
using InTheHand.Net.Sockets;
using LeHandUI;
using System.Windows.Media.Animation;
using Color = System.Windows.Media.Color;
using System.ComponentModel;

namespace LeHandUI
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    
    
    public partial class SettingsWindow : Window, INotifyPropertyChanged
    {
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
        public static SettingsWindow inst = null;
        public static SolidColorBrush clrstatus_NotConnected = new SolidColorBrush(Color.FromArgb(255, 200, 20, 45));
        public static SolidColorBrush clrstatus_Connecting = new SolidColorBrush(Color.FromArgb(255, 255, 210, 25));
        public static SolidColorBrush clrstatus_Connected = new SolidColorBrush(Color.FromArgb(255, 10, 190, 25));

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public static connectionStatus status = connectionStatus.Disconnected;
        public enum connectionStatus
        {
            Disconnected,
            Connecting,
            Connected
        }

        public SettingsWindow()
        {
            InitializeComponent();
            refreshButtonImage.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.whiteRefreshBTDevices64x64);
            inst = this;
            this.Icon = ImageSourceFromBitmap(LeHandUI.Properties.Resources.BTIcon16x16);

            
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BTGrid.ItemsSource = MainWindow.BTService.observableCollection;
            BTLog.Text = "";
        }
        static string _name2 = "";
        public static string log {
            get { return _name2; }
            set
            {
                if (value != _name2)
                {
                    _name2 = value;
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        inst.BTLog.Text = value;
                        inst.BTLog.ScrollToEnd();
                    });
                }
            }
        }
        private void button_Connect_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.BTService.latestsearch.Count > 0)
            {
                Int64 addr = MainWindow.BTService.latestsearch[BTGrid.SelectedIndex].DeviceAddress.ToInt64();
                Communicator.device.directConnect(addr);
            }
        }

        #region Bluetooth stuff
        ObservableCollection<DeviceDetails> list = new ObservableCollection<DeviceDetails>();
        
        private async void button_Refresh_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(MainWindow.BTService.SearchDevices);
            BTGrid.ItemsSource = MainWindow.BTService.observableCollection;

        }
        private async void OnSearchCompleted(Task<List<BluetoothDeviceInfo>> obj)
        {
            List<BluetoothDeviceInfo> results = await obj;
            if (results == null)
            {
                MessageBox.Show("Failed to discover Bluetooth devices, \nIs Bluetooth turned on?", "Can't find Devices", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                list.Clear();
            });
            foreach (var Result in results)
            {
                DeviceDetails dd = new DeviceDetails {
                    Name = Result.DeviceName,
                    Adress = Result.DeviceAddress.ToString()
                };
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    list.Add(dd);
                });
                
            }
            //bluetoothDeviceInfo = results;
        }
        #endregion

    }
}

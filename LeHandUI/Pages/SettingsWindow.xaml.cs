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

namespace LeHandUI.Pages
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    
    
    public partial class SettingsWindow : Window
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
        public SettingsWindow()
        {
            InitializeComponent();
            refreshButtonImage.Source = ImageSourceFromBitmap(LeHandUI.Properties.Resources.RefreshBTDevices64x64);

            BTGrid.ItemsSource = MainWindow.BTService.observableCollection;
        }
        ObservableCollection<DeviceDetails> list = new ObservableCollection<DeviceDetails>();
        List<BluetoothDeviceInfo> bluetoothDeviceInfo = null;
        
        private async void button_Refresh_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(MainWindow.BTService.SearchDevices);

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
            bluetoothDeviceInfo = results;
        }
        private void button_Connect_Click(object sender, RoutedEventArgs e)
        {
            Int64 addr = bluetoothDeviceInfo[BTGrid.SelectedIndex].DeviceAddress.ToInt64();
            Communicator.device.directConnect(addr);
        }

    }
}

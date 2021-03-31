using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using InTheHand.Net.Sockets;
using LeHandUI;

namespace LeHandUI.Pages
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    
    public class DeviceDetails
    {
        public string Name { get; set; }
        public string Adress { get; set; }
    }
    public partial class SettingsWindow : Window
    {
        ObservableCollection<DeviceDetails> list = new ObservableCollection<DeviceDetails>();
        List<BluetoothDeviceInfo> bluetoothDeviceInfo = null;
        public SettingsWindow()
        {
            InitializeComponent();
            BTGrid.ItemsSource = list;
        }
        private async void button_Refresh_Click(object sender, RoutedEventArgs e)
        {
            Task<List<BluetoothDeviceInfo>> task = Task.Run(MainWindow.BTService.SearchDevices);
            task.ContinueWith(OnSearchCompleted);

        }
        
        private async void OnSearchCompleted(Task<List<BluetoothDeviceInfo>> obj)
        {
            List<BluetoothDeviceInfo> results = await obj;
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
            button_Refresh_Click(sender, e);
        }
    }
}

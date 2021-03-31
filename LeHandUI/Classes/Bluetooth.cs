using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using InTheHand;
using InTheHand.Devices;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace LeHandUI
{
    public class DeviceDetails
    {
        public string Name { get; set; }
        public string Adress { get; set; }
    }
    public class Bluetooth
    {
        //public Bluetooth()
        //{
        //}
        public List<BluetoothDeviceInfo> latestsearch = new List<BluetoothDeviceInfo>();
        
        public ObservableCollection<DeviceDetails> observableCollection = new ObservableCollection<DeviceDetails>();
        public async void RefreshDevices()
        {
            await Task.Run(SearchDevices);
        }

        public async Task SearchDevices()
        {
            await GetBluetoothClients();
        }

        public BluetoothComponent localComponent = null;
        public async Task GetBluetoothClients()
        {
            BluetoothClient client = new BluetoothClient();
            if (InTheHand.Net.Bluetooth.BluetoothRadio.PrimaryRadio == null)
            {
                MessageBox.Show("Failed to discover Bluetooth devices", "Can't find Devices", MessageBoxButton.OK);

                return;
            }
            //client.BeginDiscoverDevices(10, false, false, true, true, null, null);
            if (localComponent != null)
            {
                localComponent.Dispose();
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    observableCollection.Clear();
                    latestsearch.Clear();
                });
            }
            localComponent = new BluetoothComponent(client);
            localComponent.DiscoverDevicesProgress += LocalComponent_DiscoverDevicesProgress;
            localComponent.DiscoverDevicesComplete += LocalComponent_DiscoverDevicesComplete;
            localComponent.DiscoverDevicesAsync(255, false, false, true, false, null);

            return;
            //return null;
        }

        private void LocalComponent_DiscoverDevicesComplete(object sender, DiscoverDevicesEventArgs e)
        {
        }

        private void LocalComponent_DiscoverDevicesProgress(object sender, DiscoverDevicesEventArgs e)
        {
            foreach (var dev in e.Devices)
            {
                DeviceDetails dd = new DeviceDetails
                {
                    Name = dev.DeviceName,
                    Adress = dev.DeviceAddress.ToString()
                };
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    observableCollection.Add(dd);
                    latestsearch.Add(dev);
                });
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InTheHand;
using InTheHand.Devices;
using InTheHand.Net.Sockets;

namespace LeHandUI
{
    public class Bluetooth
    {
        //public Bluetooth()
        //{
        //}
        public async void RefreshDevices()
        {
            await Task.Run(SearchDevices);
        }

        public async Task<List<BluetoothDeviceInfo>> SearchDevices()
        {
            List<BluetoothDeviceInfo> bluetoothDevices = await GetBluetoothClients();
            foreach (var device in bluetoothDevices)
            {
                //Int64 address = device.DeviceAddress.ToInt64();
                Debug.WriteLine(device.DeviceName);
            }
            return bluetoothDevices;
        }

        public async Task<List<BluetoothDeviceInfo>> GetBluetoothClients()
        {
            BluetoothClient client = new BluetoothClient();
            //client.BeginDiscoverDevices(10, false, false, true, true, null, null);
            return client.DiscoverDevicesInRange().ToList();
            //return null;
        }


    }
}

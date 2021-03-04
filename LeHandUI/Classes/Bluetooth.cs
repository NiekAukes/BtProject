using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InTheHand;
using InTheHand.Devices;
using InTheHand.Net.Sockets;

namespace LeHandUI
{
    class Bluetooth
    {
        public Bluetooth()
        {
            SearchDevices();
        }

        public async Task SearchDevices()
        {
            List<BluetoothDeviceInfo> bluetoothDevices = await GetBluetoothClients();
            foreach (var device in bluetoothDevices)
            {
                Debug.WriteLine(device.DeviceName);
            }
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

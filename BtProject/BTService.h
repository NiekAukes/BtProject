#pragma once
#include <WinSock2.h>
#include <ws2bth.h>
#include <bthsdpdef.h>

#include <windows.devices.bluetooth.h>
#include <windows.devices.bluetooth.advertisement.h>

#include <bluetoothapis.h>
#include <bluetoothleapis.h>
#include <windows.devices.bluetooth.background.h>
#include <windows.devices.bluetooth.genericattributeprofile.h>
#include <windows.devices.bluetooth.rfcomm.h>
#include <iostream>
#include <queue>
#include <Windows.h>

struct DeviceDetails
{
	bool valid = false;
	std::string name = "unnamed";
};

struct Axis
{
	int nAxisNum = 1;
	double val = 0;
};

union NormalData
{
	char* Raw;
	Axis* finger;
	NormalData() { Raw = new char('c'); }
};
enum class Expectedtype
{
	Protocol,
	Length,
	Data,
	Footer
};
class BTService
{

public:
	std::queue<short> Data;
	double partdata = 0;
	Expectedtype expected = Expectedtype::Protocol;
	BTService()
	{
		std::cout << "initiated service\n";
	}
	int Discover(DeviceDetails* out); //lets you discover devices
	int Connect(DeviceDetails); //allows you to connect with the device
	int ReceiveData();
	int ProcessData(NormalData* out); //receives data, returns protocol or negative if failed
	void DataGenerator();


};


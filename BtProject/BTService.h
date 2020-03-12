#ifndef BTSERVICE_H
#define BTSERVICE_H
#define _WINSOCK_DEPRECATED_NO_WARNINGS
#include <WinSock2.h>
#include <ws2bth.h>
#include <bthsdpdef.h>

#include <windows.devices.bluetooth.h>
#include <windows.devices.bluetooth.advertisement.h>

#include <bluetoothapis.h>
#include <SvcGuid.h>
#include <SetupAPI.h>
#include <bthledef.h>
#include <RegStr.h>
#include <bluetoothleapis.h>
#include <windows.devices.bluetooth.background.h>
#include <windows.devices.bluetooth.genericattributeprofile.h>
#include <windows.devices.bluetooth.rfcomm.h>
#include <iostream>
#include <queue>
#include <thread>
#include <cstdlib>
#include <bthdef.h>
#include <Windows.h>


#pragma comment(lib, "bthprops.lib")
#pragma comment(lib, "ws2_32.lib")
#pragma comment(lib, "BluetoothAPIs.lib")
#pragma comment(lib, "SetupAPI")
class BTService;
struct DeviceDetails
{
public:
	bool valid = false;
	std::string name = "unnamed";
	unsigned long long address;
	friend class BTService;

private:
	BLUETOOTH_DEVICE_INFO_STRUCT inheritData;
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
private:
	double values[11]; //values bound to protocol
	HANDLE Radio;
public:

	enum valuetype : int
	{
		Little_Finger,
		Ring_Finger,
		Middle_Finger,
		ForeFinger,
		Thumb,
		Acceleration_X,
		Acceleration_Y,
		Acceleration_Z,
		Rotation_Y
	};

	std::queue<short> Data;
	double partdata = 0;
	Expectedtype expected = Expectedtype::Protocol;


	SOCKET s;
	BTService()
	{
		std::cout << "initiated service\n";
		
	}
	int Discover(DeviceDetails** out); //lets you discover devices
	int Connect(DeviceDetails dd); //allows you to connect with the device
	int ReceiveData(char* buf, int buflen);
	int ProcessData(NormalData* out); //receives data, returns protocol or negative if failed
	void DataGenerator();

	double* getDoubleDataFromBT(int* length);


};

#endif
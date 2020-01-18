#include <iostream>

//#include <WinSock2.h>
//#include <ws2bth.h>
//#include <bthsdpdef.h>
//
//#include <windows.devices.bluetooth.h>
//#include <windows.devices.bluetooth.advertisement.h>
//
//#include <bluetoothapis.h>
//#include <windows.devices.bluetooth.background.h>
////#include <windows.devices.bluetooth.genericattributeprofile.h>
//#include <windows.devices.bluetooth.rfcomm.h>

#include "BTService.h"
int main()
{
	std::cout << "hi \n";
	BTService service;

	DeviceDetails* devices = nullptr;

	double d = 0;
	std::cin >> d;
	service.partdata = d;
	service.Discover(devices);

	std::string command;
	std::string args[10];
	
	while (1)
	{
		std::cin >> command;

		/*int currarg = 0;
		for (int i = 0; i < command.size() - 1; i++)
		{
			if (currarg >= 0)
			{
				if (command[i] == ' ')
				{
					args[currarg].append(1, '\0');
					currarg++;
					args[currarg].clear();
				}
				else
				{
					const char c = command[i];
					args[currarg].append(1, c);
				}
				if (command[i] == '\0')
				{
					break;
				}
			}
		}*/

		if (command._Equal("Discover"))
		{
			
			service.Discover(devices);
		}

		if (command._Equal("connect"))
		{
			if (devices == nullptr || devices->valid == false)
			{
				std::cout << "no devices available\n";
			}
			else
			{
				service.Connect(*devices);
			}
		}
	}

	return 0;
}


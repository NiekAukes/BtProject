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
enum condition
{
	equal,
	greaterOrEqual,
	greater,
	fewer,
	fewerOrEqual,
	notEqual
};
class Rule
{
public:
	int id = -1;
	double value = 0.5;
	condition condit;
};

int main()
{
	std::vector<Rule> ruleset;

	std::cout << "hi \n";
	BTService service;

	DeviceDetails* devices = nullptr;

	std::string command;
	
	while (1)
	{
		std::cin >> command;

		if (command._Equal("discover"))
		{
			
			service.Discover(devices);
		}

		else if (command._Equal("connect"))
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
		else if (command._Equal("set"))
		{
			std::string arg1;
			std::cin >> arg1;
			if (arg1._Equal("add"))
			{
				int arg2;
				double arg3;
				int Condition;

				std::cin >> arg2;
				std::cin >> arg3;
				std::cin >> Condition;

				Rule r = { arg2, arg3, (condition)Condition };
				ruleset.push_back(r);
			}
			if (arg1._Equal("rem"))
			{
				int arg2;
				std::cin >> arg2;
				for (int i = 0; i < ruleset.size(); i++)
				{
					if (ruleset[i].id == arg2)
					{
						ruleset.erase(ruleset.begin() + i);
					}
				}
			}

		}
		else
		{
			std::cout << "command not recognized\n";
		}
	}

	return 0;
}


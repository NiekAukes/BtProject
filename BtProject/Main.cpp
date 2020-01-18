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
typedef int condition;
std::string strcondit[6] = { "==", ">=", ">", "<", "<=", "!=" };
//enum condition
//{
//	equal,
//	greaterOrEqual,
//	greater,
//	fewer,
//	fewerOrEqual,
//	notEqual
//};
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
		else if (command._Equal("rule"))
		{
			std::cout << "add rem edit list: ";
			std::string arg1;
			std::cin >> arg1;
			if (arg1._Equal("add"))
			{
				int arg2;
				double arg3;
				int Condition;

				std::cout << "id: ";
				std::cin >> arg2;
				std::cout << "value: ";
				std::cin >> arg3;
				std::cout << "condition: ";
				std::cin >> Condition;
				std::cout << "\nrule created succesfully\n\n";

				Rule r = { arg2, arg3, (condition)Condition };
				ruleset.push_back(r);
			}
			if (arg1._Equal("rem"))
			{
				int arg2;
				std::cout << "id: ";
				std::cin >> arg2;
				std::vector<Rule> tmp; //ask for more args

				for (int i = 0; i < ruleset.size(); i++)
				{
					if (ruleset[i].id == arg2)
					{
						Rule newRule = ruleset[i];
						newRule.id = i; //set the id of the rule to the arraynumber, since the actual id is not needed in this operation.
						tmp.push_back(newRule);
					}
				}
				if (tmp.size() > 1)
				{
					//ask wich one to remove
					std::cout << "there were more rules set on this id, which one do you want to remove? \n\n";
					int ans = 0;
					for (int i = 0; i < tmp.size(); i++)
					{
						std::cout << i + 1 << '\n';
						std::cout << "Value: " << tmp[i].value << "\n Condition: " << strcondit[tmp[i].condit] << "\n";
					}
					std::cin >> ans;
					ans--;

					ruleset.erase(ruleset.begin() + tmp[ans].id);
					std::cout << "rule succesfully removed\n";
				}
				else if (tmp.size() == 1)
				{
					//just remove
				}
				else
				{
					//does not exist, so do nothing
					std::cout << "rule does not exist";
				}
			}
			else if (arg1._Equal("edit"))
			{
				int arg2;
				double arg3;
				int Condition;


				std::cin >> arg2;
				std::cin >> arg3;
				std::cin >> Condition;
			}
			else if (arg1._Equal("list"))
			{
				std::cout << '\n';
				for (int i = 0; i < ruleset.size(); i++)
				{
					std::cout << i + 1 << '\n';
					std::cout << "Id: " << ruleset[i].id << "\nValue: " << ruleset[i].value << "\nCondition: " << strcondit[ruleset[i].condit] << "\n\n";
				}
			}

		}
		else
		{
			std::cout << "command not recognized\n";
		}
		std::cout << "\n";
	}

	return 0;
}


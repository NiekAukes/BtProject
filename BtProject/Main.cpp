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
//#define WINVER 0x0500

#include "BTService.h"
#include "Keysender.h"
#include "CommandManager.h"
#include <thread>


bool active = true;
DWORD currVersion = 0.7;
std::string file;
int main(int argc, char* argv[])
{
	//check if new version is available
	if (argc > 1) {
		if (*argv[1] == '-') {
			for (int i = 0; i < argc; i++) {
				if (argv[i] == "-raw") {
					#ifndef DoRaw
					#define DoRaw
					#endif // !DoRaw

				}
				else if (argv[i] == "-file") {
					file = argv[i + 1];
				}
			}
		}
		else {
			std::cout << "loaded file: " << argv[1] << '\n';
			file = argv[1];
		}
	}
	CommandManager* cmdMgr = CommandManager::GetInst();
	cmdMgr->startcommander(false, file.size() > 1 ? file : "");
	return 0;
}


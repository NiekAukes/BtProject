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
int main(int argc, char* argv[])
{
	//check if new version is available
	if (argc > 1) {
		std::cout << "loaded file: " << argv[1] << '\n';
	}
	CommandManager* cmdMgr = CommandManager::GetInst();
	cmdMgr->startcommander(false, argc > 1 ? argv[1] : "");
	return 0;
}


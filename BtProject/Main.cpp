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

//std::string strcondit[6] = { "==", ">=", ">", "<", "<=", "!=" };
bool active = true;
int main(int argc, char* argv[])
{
	CommandManager* cmdMgr = CommandManager::GetInst();
	cmdMgr->startcommander(!(argc > 0 && argv[0] == "-res"));
	return 0;
}


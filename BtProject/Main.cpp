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
#include "Keysender.h"
#include "CommandManager.h"
#include <thread>

//std::string strcondit[6] = { "==", ">=", ">", "<", "<=", "!=" };
bool active = true;
int main()
{
	CommandManager* cmdMgr = CommandManager::GetInst();
	cmdMgr->startcommander();
	while (cmdMgr->active) {}
	return 0;
}


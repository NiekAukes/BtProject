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
#include "Logic.h"


void TestLogic() {
	//int a = 5;
	//int b = 6;
	//float c = 5.67;
	//float* d = new float(7.12);
	///*BiValue eq;
	//eq.val1 = new Expression<int>(&a, &b, Less);
	//eq.val2 = new Expression<float>(&c, d, Greater);
	//eq.type = And;*/
	//Expression<Logic> eq(0,0, And);
	//eq.val1 = new Expression<int>(&a, &b, Less);
	//eq.val2 = new Expression<float>(&c, d, Greater);

	//std::cout << eq.Value() << '\n';
	//*d = 2.87;
	//std::cout << eq.Value() << '\n';
}
bool active = true;
int main(int argc, char* argv[])
{
	TestLogic();
	if (argc > 1) {
		std::cout << "loaded file: " << argv[1] << '\n';
	}
	CommandManager* cmdMgr = CommandManager::GetInst();
	cmdMgr->startcommander(false, argc > 1 ? argv[1] : "");
	return 0;
}


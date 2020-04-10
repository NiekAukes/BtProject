#include "Keysender.h"

std::vector<Rule> Keysender::ruleset;
std::string Keysender::LuaFile;
std::string Keysender::strcondit[] = { "==", ">=", ">", "<", "<=", "!=" };
std::string protocStr[] = {
	"Little_Finger ",
	"Ring_Finger   ",
	"Middle_Finger ",
	"Index_Finger  ",
	"Thumb         ",
	"Acceleration_X",
	"Acceleration_Y",
	"Acceleration_Z",
	"Rotation_Y    "
};
bool Keysender::keysendActive = false;
int nScreenWidth = 120;
int nScreenHeight = 12;
void InsertStrInScreen(wchar_t* screen, std::string str, int pos)
{
	std::wstring wstr = std::wstring(str.begin(), str.end());
	for (int i = 0; i < str.size(); i++)
	{
		*(screen + pos + i) = wstr[i];
	}
}

void SetProtocolValue(int Protocol, double value, wchar_t* screen)
{
	int position = nScreenWidth * (Protocol + 1);
	std::stringstream stream;
	std::stringstream valstream;
	stream << Protocol;
	InsertStrInScreen(screen, stream.str(), position);
	InsertStrInScreen(screen, protocStr[Protocol - 1], position + 17);

	valstream << value;
	InsertStrInScreen(screen, valstream.str(), position + 45);
}
void Keysender::Keythreading()
{
	system("cls");
	//setup monitoring
	//get current conditions (BtService)

	LuaFTF lu;
	lu.start(LuaFile, BTService::inst);
	while (lu.running)
	{




		//configure keysender

		//update Monitoring values
		//screen

		//construct keysend
		//INPUT ip;
		//ip.type = INPUT_KEYBOARD;
		//ip.ki.wScan = 0;
		//ip.ki.time = 0;
		//ip.ki.dwExtraInfo = 0;
		//ip.ki.dwFlags = 0;

		//for (int i = 0; i < ruleset.size(); i++)
		//{
		//	Rule currRule = ruleset[i];
		//	//check if conditions apply
		//	//set key
		//	ip.ki.wVk = toupper(currRule.key);
		//	//SendInput(1, &ip, sizeof(INPUT));
		//}
		//std::cout << "Protocol            | Description                              | value\n";
		//std::cout << "--------------------+------------------------------------------+----------------------------\n";

		//for (int i = 0; i < 9; i++)
		//{
		//	std::cout << " " << i + 1 << "                  | " << protocStr[i] << "                           | " << (rand() % 1000) / 1000.0 << "\n";
		//}
		//std::this_thread::sleep_for(std::chrono::milliseconds(100));
		//system("cls");
	}

	//system("cls");
}

Keysender::Keysender(bool* f)
{
	keysendActive = true;
	keythread = new std::thread(Keythreading);
	while (true) {
		//end states, now only enter press
		if (GetKeyState(VK_RETURN) & 0x8000) {
			break;
		}
	}
	keysendActive = false;
	//system("cls");
	std::cout << "cancelled monitoring";
	//setup monitoring
	//get current conditions (BtService)
	
}

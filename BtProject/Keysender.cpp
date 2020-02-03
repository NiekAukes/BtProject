#include "Keysender.h"

std::vector<Rule> Keysender::ruleset;
std::string Keysender::strcondit[] = { "==", ">=", ">", "<", "<=", "!=" };
std::string protocStr[] = {
	"Little_Finger",
	"Ring_Finger",
	"Middle_Finger",
	"ForeFinger",
	"Thumb",
	"Acceleration_X",
	"Acceleration_Y",
	"Acceleration_Z",
	"Rotation_Y"
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
	//setup monitoring
	wchar_t* screen = new wchar_t[nScreenWidth * nScreenHeight];
	for (int i = 0; i < nScreenWidth * nScreenHeight; i++)
	{
		if (!(i > nScreenWidth - 1 && i < nScreenWidth + 60))
		{
			if (i % nScreenWidth == 40 || i % nScreenWidth == 15)
				*(screen + i) = '|';
			else
				*(screen + i) = ' ';
		}
		else
		{
			if (i % nScreenWidth == 40 || i % nScreenWidth == 15)
				*(screen + i) = '+';
			else
				*(screen + i) = '-';
		}
	}
	HANDLE hConsole = CreateConsoleScreenBuffer(GENERIC_READ | GENERIC_WRITE, 0, NULL, CONSOLE_TEXTMODE_BUFFER, NULL);
	SetConsoleActiveScreenBuffer(hConsole);
	DWORD dwBytesWritten = 0;
	
	InsertStrInScreen(screen, "Protocol", 0);
	InsertStrInScreen(screen, "Description", 17);
	InsertStrInScreen(screen, "Value", 45);
	for (int i = 0; i < 9; i++)
	{
		SetProtocolValue(i+1, (rand()%1000)/1000.0 , screen);
	}

	screen[nScreenHeight * nScreenWidth - 1] = '\0';
	WriteConsoleOutputCharacterW(hConsole, screen, nScreenWidth * nScreenHeight, { 0,0 }, &dwBytesWritten);

	//get current conditions (BtService)
	
	while (keysendActive)
	{
		//configure keysender

		//update Monitoring values
		//screen

		//construct keysend
		INPUT ip;
		ip.type = INPUT_KEYBOARD;
		ip.ki.wScan = 0;
		ip.ki.time = 0;
		ip.ki.dwExtraInfo = 0;
		ip.ki.dwFlags = 0;

		for (int i = 0; i < ruleset.size(); i++)
		{
			Rule currRule = ruleset[i];
			//check if conditions apply
			//set key
			ip.ki.wVk = toupper(currRule.key);
			//SendInput(1, &ip, sizeof(INPUT));
		}
		for (int i = 0; i < 9; i++)
		{
			SetProtocolValue(i + 1, (rand() % 1000) / 1000.0, screen);
		}
		std::this_thread::sleep_for(std::chrono::milliseconds(500));
		screen[nScreenHeight * nScreenWidth - 1] = '\0';
		WriteConsoleOutputCharacterW(hConsole, screen, nScreenWidth * nScreenHeight, { 0,0 }, &dwBytesWritten);
		INPUT_RECORD record;
		DWORD numm;
		ReadConsoleInput(hConsole, &record, 1, &numm);
		record.Event.KeyEvent.wVirtualKeyCode
	}
	//close consolebuffer
	HANDLE stdh = GetStdHandle(STD_INPUT_HANDLE);
	SetConsoleActiveScreenBuffer(stdh);

	//system("cls");
}

Keysender::Keysender(bool* f)
{
	keysendActive = true;
	keythread = new std::thread(Keythreading);
	
	/*std::cin.clear();
	char c[100];
	std::cin >> c;
	
	keysendActive = false;*/
}

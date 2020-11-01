#include "BTService.h"

typedef DeviceDetails* lpDeviceDetails;
	int BTService::Discover(DeviceDetails** out)
	{
		std::cout << "discovering\n";

		
		//WSADATA wsadat;
		//DWORD dwresult = WSAStartup(2.2, &wsadat);
		//ZeroMemory(&wsaqs, sizeof(wsaqs));
		//wsaqs.dwSize = sizeof(WSAQUERYSETW);
		//wsaqs.dwNameSpace = NS_BTH;
		//wsaqs.lpcsaBuffer = NULL;
		//wsaqs.dwNumberOfCsAddrs = 0;
		//GUID guid = SVCID_HOSTNAME;
		//wsaqs.lpServiceClassId = &guid;
		//WSASetServiceW(&wsaqs, RNRSERVICE_REGISTER, SERVICE_MULTIPLE);
		//HANDLE hLookup;
		//dwresult = WSALookupServiceBeginW(&wsaqs, LUP_CONTAINERS | LUP_FLUSHCACHE | LUP_RETURN_ADDR, &hLookup);
		//
		//

		//if (dwresult != SOCKET_ERROR) {
		//	do {
		//		char buf[4096];
		//		LPWSAQUERYSETW pwsaResults = (LPWSAQUERYSETW)buf;
		//		ZeroMemory(pwsaResults, sizeof(WSAQUERYSET));
		//		pwsaResults->dwSize = sizeof(WSAQUERYSET);
		//		pwsaResults->dwNameSpace = NS_BTH;
		//		pwsaResults->lpBlob = NULL;

		//		DWORD dwSize = sizeof(buf);
		//		dwresult = WSALookupServiceNextW(hLookup, LUP_RETURN_NAME | LUP_RETURN_ADDR, &dwSize, pwsaResults);
		//		if (dwresult != 0) {
		//			dwresult = WSAGetLastError();
		//			if ((dwresult == WSA_E_NO_MORE) || (dwresult == WSAENOMORE))
		//			{

		//				std::cout << "No more record found!\n";

		//				break;

		//			}
		//				std::cout << "WSALookupServiceNext() failed with error code \n" << WSAGetLastError();
		//			
		//		}
		//		else {
		//			dwresult = 0;
		//			std::cout << "WSALookupServiceNext() looks fine!\n";
		//		}
		//		if (dwresult == WSAEFAULT) {
		//			//invalid pointer
		//			std::cout << "invalid pointer\n";
		//			return 0;
		//		}

		//		
		//	} while ((dwresult != WSA_E_NO_MORE) && (dwresult != WSAENOMORE));
		//	WSALookupServiceEnd(hLookup);
		/*}
		else {
			dwresult = WSAGetLastError();
			std::cout << "WSALookupServiceNext() failed with error code \n" << WSAGetLastError();
		}

		SOCKET sock = socket(AF_BTH, SOCK_STREAM, BTHPROTO_RFCOMM);*/

		BLUETOOTH_FIND_RADIO_PARAMS bltFrp;
		bltFrp.dwSize = sizeof(bltFrp);
		HBLUETOOTH_RADIO_FIND hBltRf = BluetoothFindFirstRadio(&bltFrp, &Radio);
		BOOL hasNext;
		int RadioAmount = 0;
		do {
			if (hBltRf != NULL) 
				RadioAmount++;
			hasNext = BluetoothFindNextRadio(hBltRf, &Radio);
		} while (hasNext);
		BluetoothFindRadioClose(hBltRf);

		BLUETOOTH_RADIO_INFO bltRi;
		bltRi.dwSize = sizeof(bltRi);
		DWORD info = BluetoothGetRadioInfo(Radio, &bltRi);

		if (RadioAmount == 1) {
			BLUETOOTH_DEVICE_SEARCH_PARAMS bltDsp;
			bltDsp.hRadio = Radio;
			bltDsp.fIssueInquiry = true;
			bltDsp.fReturnAuthenticated = true;
			bltDsp.fReturnConnected = false;
			bltDsp.fReturnRemembered = false;
			bltDsp.fReturnUnknown = true;
			bltDsp.cTimeoutMultiplier = 4;
			bltDsp.dwSize = sizeof(bltDsp);
			BLUETOOTH_DEVICE_INFO_STRUCT bltDis[20];
			for (int i = 0; i < 20; i++) {
				(bltDis + i)->dwSize = sizeof(*(bltDis + i));
			}
			HBLUETOOTH_DEVICE_FIND hBltDf = BluetoothFindFirstDevice(&bltDsp, bltDis);
			int DeviceAmount = 0;
			std::vector<DeviceDetails> Devicelist;
			if (hBltDf != NULL) {
				do {
					hasNext = BluetoothFindNextDevice(hBltDf, (bltDis + DeviceAmount));
						DeviceAmount++;

						//print device name
						char str[248];
						size_t* s = new size_t(249);
						wcstombs_s(s, str, (bltDis + DeviceAmount - 1)->szName, 248);
						delete s;
						std::cout << DeviceAmount << ": " << str << "  |  ";
						std::cout/* << std::hex*/ << (bltDis + DeviceAmount - 1)->Address.ullLong <<'\n';

						//register device to BTService
						DeviceDetails details;
						details.address = (bltDis + DeviceAmount - 1)->Address.ullLong;
						details.inheritData = *(bltDis + DeviceAmount - 1);
						details.name = str;
						details.valid = true;
						Devicelist.push_back(details);

				
				} while (hasNext && DeviceAmount < 20);

				BluetoothFindDeviceClose(hBltDf);

				DeviceDetails* devicels = new DeviceDetails[Devicelist.size()];
				for (int i = 0; i < Devicelist.size(); i++) {
					*(devicels + i) = Devicelist[i];
				}
				*out = devicels;
				return DeviceAmount;
			}
			else {
				DWORD dwError = GetLastError();
				if (dwError != ERROR_NO_MORE_ITEMS)
					std::cout << dwError;
				else //nothing to worry about
					std::cout << "no devices found\n";
			}
		}


		return 0;
	}

	bool receivedCallback = false;
	BOOL CALLBACK bluetoothAuthCallback(LPVOID param, PBLUETOOTH_AUTHENTICATION_CALLBACK_PARAMS params)
	{
		BLUETOOTH_AUTHENTICATE_RESPONSE response;
		::ZeroMemory(&response, sizeof(BLUETOOTH_AUTHENTICATE_RESPONSE));
		response.authMethod = params->authenticationMethod;
		response.bthAddressRemote = params->deviceInfo.Address;
		response.negativeResponse = FALSE;
		DWORD error = ::BluetoothSendAuthenticationResponseEx(nullptr, &response);

		//std::cout << "callback happened" << '\n';
		receivedCallback = true;
		return TRUE;
	}


	/*
		-Code 1: Callback Failed
		-Code 2: Pairing failed due to error
		-Code 3: Could not start WSA
		-Code 4: Connecting failed
		-Code 5: Socket binding failed
	*/
	int BTService::Connect(DeviceDetails dd)
	{
		DWORD dwResult;
		if (!dd.paired) {
			std::cout << "attempting to connect to device\n";
			HBLUETOOTH_AUTHENTICATION_REGISTRATION hCallbackHandle = 0;
			dwResult = BluetoothRegisterForAuthenticationEx(
				&dd.inheritData,
				&hCallbackHandle,
				(PFN_AUTHENTICATION_CALLBACK_EX)&bluetoothAuthCallback,
				NULL);
			if (dwResult != ERROR_SUCCESS) {
				std::cout << "failed callback " << GetLastError() << '\n';
				return 1;
			}
			std::string code;
			DWORD res;
			dwResult = BluetoothAuthenticateDeviceEx(NULL, NULL, &dd.inheritData, NULL, MITMProtectionNotRequired);
			DWORD newdwResult = BluetoothUnregisterAuthentication(hCallbackHandle);
			switch (dwResult)
			{
			case ERROR_SUCCESS:
				std::cout << "pair device success" << std::endl;
				break;

			case ERROR_CANCELLED:
				std::cout << "pair device failed, user cancelled" << std::endl;
				return 2;
				break;

			case ERROR_INVALID_PARAMETER:
				std::cout << "pair device failed, invalid parameter" << std::endl;
				return 2;
				break;

			case ERROR_NO_MORE_ITEMS:
				std::cout << "device appears paired already" << std::endl;
				break;
			case WAIT_TIMEOUT:
				//std::cin >> code;
				//res = BluetoothAuthenticateDeviceEx(NULL, NULL, &dd.inheritData, NULL, MITMProtectionRequired);
				std::cout << "device timed out, continuing connection" << std::endl;
				break;

			default:
				std::string s;
				std::cout << "pair device failed, unknown error, code " << (unsigned int)dwResult << std::endl;
				break;
			}
			
		}

		WSADATA wsadat;
		WSAQUERYSETW wsaqs;
		ZeroMemory(&wsadat, sizeof(wsadat));
		ZeroMemory(&wsaqs, sizeof(wsaqs));
		wsaqs.dwNameSpace = NS_BTH;
		wsaqs.dwSize = sizeof(WSAQUERYSETW);

		dwResult = WSAStartup(2.2, &wsadat);
		if (dwResult != SOCKET_ERROR) {

			s = socket(AF_BTH, SOCK_STREAM, BTHPROTO_RFCOMM); //was L2CAP 

			SOCKADDR_BTH name;
			ZeroMemory(&name, sizeof(SOCKADDR_BTH));
			name.addressFamily = AF_BTH;
			name.btAddr = 0;
			name.serviceClassId = GUID_NULL;
			name.port = 0;
			dwResult = bind(s, (sockaddr*)&name, sizeof(SOCKADDR_BTH));

			if (dwResult != 0) {
				std::cout << "something went wrong: " << WSAGetLastError();
				return 5;
			}

			

			SOCKADDR_BTH sAddrBth;
			ZeroMemory(&sAddrBth, sizeof(SOCKADDR_BTH));
			sAddrBth.addressFamily = AF_BTH;
			sAddrBth.btAddr = dd.inheritData.Address.ullLong;
			sAddrBth.serviceClassId = (GUID)SerialPortServiceClass_UUID;
			sAddrBth.port = 0;
			dwResult = connect(s, (sockaddr*)&sAddrBth, sizeof(SOCKADDR_BTH)); //this needs to be changed
			if (dwResult != SOCKET_ERROR) {
				//succeeded in connected
				std::cout << "now connected to the device" << '\n';
				
				//register latest address
				HKEY hkey;
				DWORD Dispos;
				//create key
				LSTATUS stat = RegCreateKeyEx(HKEY_CURRENT_USER,
					TEXT("Software\\LeHand"), 0, NULL, 0,
					KEY_WRITE, 0, &hkey, &Dispos);

				if (stat != ERROR_SUCCESS) {
					std::cout << "failed to update key: " << stat;
				}
				else {
					unsigned long long address = dd.inheritData.Address.ullLong;
					RegSetValueEx(hkey,
						"LastAddress", 0, REG_QWORD, (const BYTE*)&address, sizeof(address));
					RegCloseKey(hkey);
				}


				ReceiveData(NULL, 0);
			}
			else {
				std::cout << "Failed to connect: " << WSAGetLastError() << '\n';
				return 4;
			}

		}
		else {
			std::cout << "WSAStartUp Failed" << dwResult << '\n';
			return 3;
		}

		
		return 0;
	}

	int BTService::LatestConnect()
	{
		HKEY hkey;
		LONG lRes = RegOpenKeyExA(HKEY_CURRENT_USER, "Software\\LeHand", 0, KEY_READ, &hkey);
		if (lRes == ERROR_SUCCESS) {
			//succeeded in opening key
			ULONG64 address;
			DWORD dwBufsize = sizeof(address);
			lRes = RegQueryValueExA(hkey, "LastAddress", NULL, NULL, (LPBYTE)&address, &dwBufsize);
			if (lRes == ERROR_SUCCESS) {
				//success, proceed with connection
				DeviceDetails devDet;
				devDet.address = address;
				devDet.inheritData.Address.ullLong = address;
				devDet.paired = true;
				devDet.valid = true;
				Connect(devDet);
			}
		}
		else {
			if (lRes == ERROR_FILE_NOT_FOUND)
				std::cout << "could not retrieve address, try connecting manually";
			else
				std::cout << "failed to open key: " << lRes;
		}
		return 0;
	} 

	void receiver(bool* signal, SOCKET s, BTService* bts) 
	{
		while (signal == nullptr ? false : *signal) 
		{
			char buf[8] = "       "; //creates buffer
			recv(s, buf, 8, 0); //receives values from bt
			for (int i = 0; i < 8; i++) { //loops through all
				if (buf[i] != ' ') { //if character is not space, filter it
					std::cout << buf[i]; //print
					bts->Data.push(buf[i]); //push on the Data Stack

				}
				if (buf[i] == 0xff && buf[i + 1] == 0xff) { //if footer detected
					bts->Data.push(buf[i]);
					bts->Data.push(buf[i+1]);
					bts->ProcessData(bts->dat); //process all data
					bts->ApplyData(bts->dat, true);
					bts->dat = nullptr;

					//signal that data has been processed: CHECK

					i++;
				}
			}
		}
		std::cout << "stopped receiving: " << (signal == nullptr ? "signal corrupt" : "ended");
	}

	bool* BTService::ReceiveData(char* buf, int buflen)
	{
		bool* sig = new bool(true);
		std::thread* recvThread = new std::thread(receiver, sig, s, this);
		return sig;
	}


	int BTService::ProcessData(NormalData* out)
	{
		/*
			Data format: uses 16 bit data chunks
				--Header
				0x0008 (Protocol for identifying type)
				0x0003 (Length of data)

				--Data
				0xA064
				0x74B4
				etc...

				--Footer
				0xFFFF


		*/
		short Length = -1;
		short Protocol = -1;
		std::vector<short>* ret = new std::vector<short>();

		//dequeue and put in vector
		int i = 0;
		while (Data.size() > 0) {
			short chunk = Data.front();
			ret->push_back(chunk);
			Data.pop();

			if (i == 0)
				Protocol = chunk;

			if (i == 1)
				Length = chunk;

			if (i == Length + 2) { //footer expected
				if (chunk = 0xFFFF) {
					break;
				}
				else return 1;
			}
			i++;
		}
		out = (NormalData*)(ret->data());
		//short databuf[4] = {0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF};
		//while (Active)
		//{
		//	if (Data.size() != 0)
		//	{
		//		unsigned short chunk = Data.front(); Data.pop();
		//
		//		if (expected == Expectedtype::Protocol)
		//		{
		//			if (chunk < 32)
		//			{
		//				//normal
		//				Protocol = chunk;
		//				if (chunk < 12)
		//				{
		//					//ret.finger = new Axis();
		//					//ret.finger->nAxisNum = chunk - 2;
		//				}
		//				else if (chunk < 32)
		//				{
		//					//other undefined
		//				}
		//				expected = Expectedtype::Length;
		//			}
		//			//out of range
		//		}
		//		else if (expected == Expectedtype::Length)
		//		{
		//			Length = chunk;
		//			expected = Expectedtype::Data;
		//		}
		//		else if (expected == Expectedtype::Data)
		//		{
		//			if (chunk != (unsigned short)(0xFFFF)) //not a footer
		//			{
		//				if (Protocol < 7)
		//				{
		//					//data is of double type
		//					for (int p = 0; p < 4; p++)
		//					{
		//						if (databuf[p] == (short)0xFFFF)
		//						{
		//							//free space
		//							databuf[p] = chunk;
		//							break;
		//						}
		//					}
		//
		//				}
		//			}
		//			else
		//			{
		//				//do final things and return
		//
		//				expected = Expectedtype::Protocol;
		//
		//				if (Protocol < 12)
		//				{
		//					double* value = new double(0);
		//					for (int p = 0; p < 4; p++) //convert data buffer to double
		//					{
		//						if (databuf[p] != 0xFFFF)
		//						{
		//							*((short*)value + p) = databuf[p];
		//						}
		//						else break;
		//					}
		//					//ret.finger->val = *value;
		//					if (out != nullptr)
		//						//*out = ret;
		//					return 0;
		//				}
		//			}
		//		}
		//
		//	}
		//	else
		//	{
		//		return 1;
		//	}
		//}

		return 0;
	}

	void BTService::ApplyData(NormalData* DataIn, bool Del = false)
	{
		//set the data in the right place
		if (DataIn->id < 11) {
			values[DataIn->id] = DataIn->data.Double;
		}
		if (Del)
			delete DataIn;
	}

	int BTService::DataGenerator()
	{
		for (int k = 0; k < 1; k++)
		{
			//int n = (rand() * 36 + 4) % 40;
			int n = 7;

			//Header
			Data.push(0x0001);
			Data.push(n - 3);
			//Data
			double val = (rand() % 1) * 124.57809245982;
			for (int i = 0; i < n - 3; i++)
			{
				Data.push(*((short*)&val + i));
				std::cout << *((short*)&val + i) << '\n';
			}
			std::cout << '\n';
			//Footer
			Data.push(0xFFFF);

			NormalData* returnad = new NormalData();
			ProcessData(returnad);
			std::cout << returnad->data.Double << '\n';
			return 0;
		}
	}

	double fRand(double fMin, double fMax)
	{
		double f = (double)rand() / RAND_MAX;
		return fMin + f * (fMax - fMin);
	}
	double iRand(int Min, int Max)
	{
		double f = rand() / RAND_MAX;
		return (int)(Min + f * (Max - Min));
	}
	int ider = 0;
	double lastval[11];
	std::vector<short> BTService::GetGeneratedData()
	{
		
		std::vector<short> vshort;
		for (int k = 0; k < 1; k++)
		{
			//int n = (rand() * 36 + 4) % 40;
			int n = 7;

			//Header
			vshort.push_back(ider);
			vshort.push_back(n - 3);
			//Data
			double val = fRand(-0.2,0.2);
			lastval[ider] += val;
			val = lastval[ider];

			for (int i = 0; i < n - 3; i++)
			{
				vshort.push_back(*((short*)&val + i));
			}
			//std::cout << "sent: " << val << '\n';
			//Footer
			vshort.push_back(0xFFFF);

			if (ider > 9)
				ider = 0;
			else
				ider++;
			return vshort;
		}
	}

	double* BTService::getDoubleDataFromBT(int* length)
	{
		for (int i = 0; i < 11; i++) {
			*(values + i) = rand();
		}
		*length = 11;
		return values;
	}
	BTService* BTService::inst;
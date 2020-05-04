#include "CommandManager.h"
#include <limits>
#undef min
#undef max
CommandManager* CommandManager::inst = nullptr;

int convchar2int(char c[2])
{
	if (c[1] == 0) //chars ended there
	{
		if (c[0] == '>')
			return 2;
		else if (c[0] == '<')
			return 3;
		else std::cout << "invalid argument: condition";
	}
	else
	{
		if (c[0] == '=')
			return 0;
		else if (c[0] == '>')
			return 1;
		else if (c[0] == '<')
			return 4;
		else if (c[0] == '!')
			return 5;
		else std::cout << "invalid argument: condition";
	}
}

void CommandManager::starter()
{
	startcommander(true);
	active = false;
}
int pos;
void getval(std::string file, char* out, int len) {
	std::string dat = file.substr(pos, len);
	pos += len;
	for (int i = 0; i < len; i++) {
		*(out + i) = dat[i];
	}
}

void CommandManager::loadbtdfile(std::string arg1) {
	std::ifstream sav;
	sav.open(arg1, std::ios::binary);
	Keysender::ruleset.clear();
	pos = 0;
	std::string save((std::istreambuf_iterator<char>(sav)),
		std::istreambuf_iterator<char>());
	
	size_t rulesize;
	getval(save, (char*)&rulesize, 4);
	for (int i = 0; i < rulesize; i++) {
		Rule f;
		int partsize;
		getval(save, (char*)&partsize, 4);
		getval(save, (char*)&f.id, 4);
		getval(save, (char*)&f.key, 1);
		for (int j = 0; j < partsize; j++) {
			Rulepart rP;
			getval(save, (char*)&rP.id, 4);
			getval(save, (char*)&rP.condit, 4);
			getval(save, (char*)&rP.value, 8);
			short corrector;
			getval(save, (char*)&corrector, 2);
			if (corrector != 0x0200) {//bad thing
				std::cout << "bad thing happend with rulepart";
				Keysender::ruleset.clear();
				break;
			}
			f.parts.push_back(rP);
		}
		short rulecorrector;
		getval(save, (char*)&rulecorrector, 2);
		if (rulecorrector != 0x0100) {//bad thing
			std::cout << "bad thing happend with rule";
			Keysender::ruleset.clear();
			break;
		}
		Keysender::ruleset.push_back(f);
	}
	sav.close();
	return;
}


void doData(BTService service, Keysender* keysend) {
	while (1) {
		std::vector<short> dat = service.GetGeneratedData();

		short* sdat = new short[dat.size()];
		for (int i = 0; i < dat.size(); i++) {
			*(sdat + i) = dat[i];
		}

		WriteFile(keysend->datapipe, (char*)sdat, dat.size() * 2, &keysend->dwdataread, NULL);
		std::this_thread::sleep_for(std::chrono::milliseconds(50));
	}
}

void CommandManager::startcommander(bool intro, std::string loadfile)
{
	if (true || std::this_thread::get_id() == commandthread->get_id())
	{
		if (intro)
			std::cout << "hi \n";

		if (loadfile != "") {
#ifdef Obsolete
			loadbtdfile(loadfile);
#else
			Keysender::LuaFile = loadfile;
#endif
		}
		BTService service;

		//set title
		SetConsoleTitleW(L"LeHand");

		//setup pipes
		keysend->datapipe = CreateNamedPipe(TEXT("\\\\.\\pipe\\LeHandData"), PIPE_ACCESS_DUPLEX, PIPE_TYPE_BYTE | PIPE_READMODE_BYTE | PIPE_WAIT,
			1, 1024 * 16, 1024 * 16, NMPWAIT_USE_DEFAULT_WAIT, NULL);
		keysend->errorpipe = CreateNamedPipe(TEXT("\\\\.\\pipe\\LeHandError"), PIPE_ACCESS_DUPLEX, PIPE_TYPE_BYTE | PIPE_READMODE_BYTE | PIPE_WAIT,
			1, 1024 * 16, 1024 * 16, NMPWAIT_USE_DEFAULT_WAIT, NULL);
		
		
		std::string command;

		std::thread datathread(doData, service, keysend);

		while (1)
		{
			if (std::cin.fail()) {
				auto state = std::cin.rdstate();
				std::cin.ignore(std::numeric_limits<std::streamsize>::max(), '\n');
				std::cin.clear();
				state = std::cin.rdstate();
				std::cin.ignore(std::numeric_limits<std::streamsize>::max(), '\n');

			}
			char f = std::cin.peek();
			std::cin >> command;
			if (*command.c_str() != '\0') {
				if (command._Equal("rule"))
				{
#ifdef Obsolete
					if (std::cin.peek() == 10)
						std::cout << "specify action: add rem edit list\n";
					std::string arg1;
					std::cin >> arg1;

					if (arg1._Equal("add"))
					{
						//create more args
						int arg2;
						double arg3;
						char key;
						char getcondit[2];
						int Condition = 0;

						if (std::cin.peek() == 10)
							std::cout << "id: ";
						std::cin >> arg2;
						if (std::cin.peek() == 10)
							std::cout << "value: ";
						std::cin >> arg3;
						if (std::cin.peek() == 10)
							std::cout << "key: ";
						std::cin >> key;

						if (std::cin.peek() == 10)
							std::cout << "condition: ";
						std::cin >> getcondit;

						//convert char condition to int condition
						Condition = convchar2int(getcondit);

						bool exist = false;
						for (int i = 0; i < Keysender::ruleset.size(); i++)
						{

							if (Keysender::ruleset[i].id == arg2)
							{
								//check if ruleset exists
								std::cout << "failed to create rule, there was already a rule there";
								exist = true;
							}
						}
						if (!exist)
						{
							Rulepart r = { arg2, arg3, (condition)Condition };
							Rule rule;
							rule.id = arg2;
							rule.key = key;
							rule.parts.push_back(r);
							Keysender::ruleset.push_back(rule);
							std::cout << "\nrule created succesfully\n";
						}
					}
					if (arg1._Equal("rem"))
					{
						int arg2;
						if (std::cin.peek() == 10)
							std::cout << "id: ";
						std::cin >> arg2;
						std::vector<Rule> tmp; //ask for more args

						for (int i = 0; i < Keysender::ruleset.size(); i++)
						{
							if (Keysender::ruleset[i].id == arg2)
							{
								Rule newRule = Keysender::ruleset[i];
								newRule.id = i; //set the id of the rule to the arraynumber, since the actual id is not needed in this operation.
								tmp.push_back(newRule);
							}
						}
						/*if (tmp.size() > 1)
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
						} */
						if (tmp.size() == 1)
						{
							//just remove (ToDo)
							Keysender::ruleset.erase(Keysender::ruleset.begin() + tmp[0].id);
							std::cout << "rule removed succesfully\n";
						}
						else
						{
							//does not exist, so do nothing
							std::cout << "rule does not exist or multiple rules were found\n";
						}
					}
					else if (arg1._Equal("edit"))
					{
						int arg2; //rule Id
						double arg3; //value
						char Condition[2];
						int arg4 = 1; //rulepart Id

						if (std::cin.peek() == 10)
							std::cout << "rule Id: ";
						std::cin >> arg2;
						if (std::cin.peek() == 10)
							std::cout << "value: ";
						std::cin >> arg3;
						if (std::cin.peek() == 10)
							std::cout << "new condition, [rulepart-Id]: ";
						std::cin >> Condition;
						if (std::cin.peek() != 10) //optional var
							std::cin >> arg4;

						//todo

						//search Rule and Rulepart
						if (Keysender::ruleset.size() >= arg2)
						{
							if (Keysender::ruleset[arg2 - 1].parts.size() >= arg4)
							{
								Rulepart* part = &Keysender::ruleset[arg2 - 1].parts[arg4 - 1];
								part->value = arg3;
								part->condit = convchar2int(Condition);
								std::cout << "Edited Rulepart\n";
							}
							else
							{
								//create new rule
								Rulepart newPart;
								newPart.id = Keysender::ruleset[arg2 - 1].parts.size();
								newPart.value = arg4;
								newPart.condit = convchar2int(Condition);
								Keysender::ruleset[arg2 - 1].parts.push_back(newPart);
								std::cout << "Created new Rulepart\n";
							}
						}
						else
						{
							std::cout << "Invalid argument: Id";
						}
					}
					else if (arg1._Equal("list"))
					{
						std::cout << '\n';
						for (int i = 0; i < Keysender::ruleset.size(); i++)
						{
							std::cout << "Rule: " << i + 1 << '\n';
							for (int j = 0; j < Keysender::ruleset[i].parts.size(); j++)
							{
								std::cout << "\trulepart: " << j + 1 << '\n';
								std::cout << "\t\tId: " << Keysender::ruleset[i].parts[j].id << "\n\t\tValue: " << Keysender::ruleset[i].parts[j].value << "\n\t\tCondition: " << Keysender::strcondit[Keysender::ruleset[i].parts[j].condit] << "\n\n";

							}
						}
					}
				}
#else
					std::cout << "out of date, use Lua instead\n";
#endif

				}
				else if (command._Equal("device"))
				{
					std::string arg2;
					if (std::cin.peek() == 10)
						std::cout << "specify action: discover connect list\n";
					std::cin >> arg2;
					if (arg2._Equal("discover"))
					{
						int ndev = service.Discover(&devices);
						deviceLen = ndev;
					}

					else if (arg2._Equal("connect"))
					{
						if (devices == nullptr || devices->valid == false)
						{
							std::cout << "no devices available\n";
						}
						else
						{
							if (deviceLen == 1) {
								service.Connect(*devices);
							}
							else {
								int arg3;
								if (std::cin.peek() == 10)
									std::cout << "device: ";
								std::cin >> arg3;
								if (arg3 < deviceLen + 1)
									service.Connect(*(devices + arg3));
								else
									std::cout << "connect failed: ERROR_INVALID_ARGUMENT\n";
							}
						}
					}
					else if (arg2._Equal("list") || arg2._Equal("ls"))
					{
						//to do
						std::cout << "Not yet implemented\n";
					}
					else if (arg2._Equal("auto")) {
						int ret = service.LatestConnect();
					}
				}
				else if (command._Equal("start"))
				{
					keysend->startSender();
					command = "";

					//std::this_thread::sleep_for(std::chrono::microseconds(500));
					std::cout << "commandline active\n";
				}

				else if (command._Equal("save")) {
#ifdef Obsolete
					std::string skip;
					std::string arg1;
					if (std::cin.peek() != 10) //optional var
						std::getline(std::getline(std::cin, skip, '"'), arg1, '"');

					std::ofstream save;
					if (arg1.empty()) {
						save.open("newsave.btd", std::ios::binary | std::ios::out | std::ios::trunc);
					}
					else {

						save.open(arg1.c_str(), std::ios::binary | std::ios::out | std::ios::trunc);
					}
					size_t siz = Keysender::ruleset.size();
					save.write((char*)&siz, 4);
					for (int i = 0; i < Keysender::ruleset.size(); i++) {
						Rule f = Keysender::ruleset[i];
						siz = f.parts.size();
						save.write((char*)&siz, 4);
						save.write((char*)&f.id, 4);
						save.write((char*)&f.key, 1);
						for (int j = 0; j < f.parts.size(); j++) {
							save.write((char*)&f.parts[j].id, 4);
							save.write((char*)&f.parts[j].condit, 4);
							save.write((char*)&f.parts[j].value, 8);
							save.put(0x00); save.put(0x02);
						}
						save.put(0x00); save.put(0x01);
					}
					save.close();
#else std::cout << "Obsolete command";
#endif
				}
				else if (command._Equal("load")) {

					std::string skip;
					std::string arg1;
					std::getline(std::getline(std::cin, skip, '"'), arg1, '"');
#ifdef Obsolete
					loadbtdfile(arg1);
#else 
					Keysender::LuaFile = arg1;
#endif
				}
				//else if (command._Equal("pipe")) {
				//	std::string arg1; //pipeName
				//	std::string arg2 = ""; //pipe function

				//	if (arg2 == "error") {

				//	}
				//	else if (arg2 == "data") {

				//	}
				//	else {
				//		std::cout << "function not found";
				//	}
				//}
				else if (command._Equal("quit"))
				{
					return;
				}
				else if (command._Equal("help") || command._Equal("--help"))
				{
					//todo
					std::cout << "Command\t\t | Args\t\t\t\t\t | Description\n";
					std::cout << "help\t\t | None \t\t\t\t | provides help\n";
					std::cout << "rule add\t | <Id> <Value> <key> <Condit>\t | creates new rule\n";
					std::cout << "rule rem\t | <Id> \t\t\t\t | removes rule\n";
					std::cout << "rule edit\t | <Id> <NewValue> <NewCondit> [Part-Id = 1] | edit rule\n";
					std::cout << "rule list\t | None\t\t\t\t\t | lists rules\n";
					std::cout << "save\t\t | [filename]\t\t\t\t | saves file\n";
					std::cout << "load\t\t | <filename>\t\t\t\t | loads file\n";
					std::cout << "device discover\t | <Amount> \t\t\t\t | Discovers BTDevices\n";
					std::cout << "device connect\t | <Id> \t\t\t\t | Connects to the device\n";
					std::cout << "device auto\t | None \t\t\t\t | Discovers and connects automatically with the device\n";
				}
				else
				{
					//std::cout << "command not recognized\n";
					system(command.c_str());
				}
			}
				else return;
			std::cout << "\n";
		}
	}
}

void CommandManager::stopcommander()
{
	if (commandthread != nullptr)
	{
		commandthread->~thread();
	}
}

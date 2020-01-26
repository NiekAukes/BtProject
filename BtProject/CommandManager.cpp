#include "CommandManager.h"
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
	startcommander();
	active = false;
}

void CommandManager::startcommander()
{
	if (std::this_thread::get_id() == commandthread->get_id())
	{

		std::cout << "hi \n";
		BTService service;

		DeviceDetails* devices = nullptr;

		std::string command;


		while (1)
		{
			std::cin >> command;

			if (command._Equal("rule"))
			{
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
						std::cout << "\nrule created succesfully\n\n";
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
					int arg3; //rulepart Id
					double arg4; //value
					char Condition[2];

					std::cin >> arg2;
					std::cin >> arg3;
					std::cin >> arg4;
					std::cin >> Condition;

					//todo

					//search Rule and Rulepart
					if (Keysender::ruleset.size() >= arg2)
					{
						if (Keysender::ruleset[arg2 - 1].parts.size() >= arg3)
						{
							Rulepart* part = &Keysender::ruleset[arg2 - 1].parts[arg3 - 1];
							part->value = arg4;
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
			else if (command._Equal("device"))
			{
				std::string arg2;
				if (std::cin.peek() == 10)
					std::cout << "specify action: discover connect list\n";
				std::cin >> arg2;
				if (arg2._Equal("discover"))
				{
					service.Discover(devices);
				}

				else if (arg2._Equal("connect"))
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
				else if (arg2._Equal("list") || arg2._Equal("ls"))
				{
					//to do
					std::cout << "Not yet implemented\n";
				}
			}
			else if (command._Equal("start"))
			{
				keysend = new Keysender();
				std::cin.clear();
			}
			else if (command._Equal("quit"))
			{
				return;
			}
			else if (command._Equal("help") || command._Equal("--help"))
			{
				//todo
				std::cout << "Command\t\t | Args\t\t\t\t\t | Description\n";
				std::cout << "rule add\t | <Id> <Value> <Condit>\t\t | creates new rule\n";
				std::cout << "rule rem\t | <Id> \t\t\t\t | removes rule\n";
				std::cout << "rule edit\t | <Id> <Part-Id> <NewValue> <NewCondit> | edit rule\n";
				std::cout << "rule list\t | None\t\t\t\t\t | lists rules\n";
				std::cout << "device discover\t | <Amount> \t\t\t\t | Discovers BTDevices\n";
				std::cout << "device connect\t | <Id> \t\t\t\t | Connects to the device\n";
				std::cout << "device auto\t | None \t\t\t\t | Discovers and connects automatically with the device\n";
			}
			else
			{
				std::cout << "command not recognized\n";
			}
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

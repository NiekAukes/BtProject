#include "CommandManager.h"
CommandManager* CommandManager::inst = nullptr;


void CommandManager::starter()
{
	startcommander();
	active = false;
}

void CommandManager::startcommander()
{
	if (std::this_thread::get_id() == commandthread->get_id())
	{
		std::vector<Rule> ruleset;

		std::cout << "hi \n";
		BTService service;

		DeviceDetails* devices = nullptr;

		std::string command;


		while (1)
		{
			std::cin >> command;

			if (command._Equal("discover"))
			{

				service.Discover(devices);
			}

			else if (command._Equal("connect"))
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
			else if (command._Equal("rule"))
			{
				if (std::cin.peek() == 10)
					std::cout << "specify action: add rem edit list\n";
				std::string arg1;
				std::cin >> arg1;
				if (arg1._Equal("add"))
				{
					int arg2;
					double arg3;
					char getcondit[2];
					int Condition = 0;
					if (std::cin.peek() == 10)
						std::cout << "id: ";
					std::cin >> arg2;
					if (std::cin.peek() == 10)
						std::cout << "value: ";
					std::cin >> arg3;
					if (std::cin.peek() == 10)
						std::cout << "condition: ";
					std::cin >> getcondit;
					if (getcondit[1] == 0) //chars ended there
					{
						if (getcondit[0] == '>')
							Condition = 2;
						else if (getcondit[0] == '<')
							Condition = 3;
						else std::cout << "invalid argument: condition";
					}
					else
					{
						if (getcondit[0] == '=')
							Condition = 0;
						else if (getcondit[0] == '>')
							Condition = 1;
						else if (getcondit[0] == '<')
							Condition = 4;
						else if (getcondit[0] == '!')
							Condition = 5;
						else std::cout << "invalid argument: condition";
					}


					for (int i = 0; i < ruleset.size(); i++)
					{
						if (ruleset[i].id == arg2)
						{
							//check if ruleset exists
							std::cout << "failed to create rule, there was already a rule there";
							return;
						}
					}

					Rulepart r = { arg2, arg3, (condition)Condition };
					Rule rule;
					rule.id = arg2;
					rule.parts.push_back(r);
					ruleset.push_back(rule);
					std::cout << "\nrule created succesfully\n\n";
				}
				if (arg1._Equal("rem"))
				{
					int arg2;
					std::cout << "id: ";
					std::cin >> arg2;
					std::vector<Rule> tmp; //ask for more args

					for (int i = 0; i < ruleset.size(); i++)
					{
						if (ruleset[i].id == arg2)
						{
							Rule newRule = ruleset[i];
							newRule.id = i; //set the id of the rule to the arraynumber, since the actual id is not needed in this operation.
							tmp.push_back(newRule);
						}
					}
					//if (tmp.size() > 1)
					//{
					//	//ask wich one to remove
					//	std::cout << "there were more rules set on this id, which one do you want to remove? \n\n";
					//	int ans = 0;
					//	for (int i = 0; i < tmp.size(); i++)
					//	{
					//		std::cout << i + 1 << '\n';
					//		std::cout << "Value: " << tmp[i].value << "\n Condition: " << strcondit[tmp[i].condit] << "\n";
					//	}
					//	std::cin >> ans;
					//	ans--;

					//	ruleset.erase(ruleset.begin() + tmp[ans].id);
					//	std::cout << "rule succesfully removed\n";
					//}
					if (tmp.size() == 1)
					{
						//just remove
					}
					else
					{
						//does not exist, so do nothing
						std::cout << "rule does not exist or multiple rules were found";
					}
				}
				else if (arg1._Equal("edit"))
				{
					int arg2;
					double arg3;
					int Condition;


					std::cin >> arg2;
					std::cin >> arg3;
					std::cin >> Condition;
				}
				else if (arg1._Equal("list"))
				{
					std::cout << '\n';
					for (int i = 0; i < ruleset.size(); i++)
					{
						std::cout << "Rule: " << i + 1 << '\n';
						for (int j = 0; j < ruleset[i].parts.size(); j++)
						{
							std::cout << "\trulepart: " << j + 1 << '\n';
							std::cout << "\t\tId: " << ruleset[i].parts[j].id << "\n\t\tValue: " << ruleset[i].parts[j].value << "\n\t\tCondition: " << Keysender::strcondit[ruleset[i].parts[j].condit] << "\n\n";
						}
					}
				}

			}
			else if (command._Equal("start"))
			{
				keysend = new Keysender();
			}
			else if (command._Equal("quit"))
			{
				return;
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

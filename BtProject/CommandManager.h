#pragma once
#include <thread>
#include <vector>
#include <iostream>
#include "Keysender.h"
#include "BTService.h"
class CommandManager
{
private:
	CommandManager() {}
	Keysender* keysend = nullptr;
	std::thread* commandthread = nullptr;
	static CommandManager* inst;
	void starter();
public:
	bool active = false;
	static CommandManager* GetInst()
	{
		if (inst == nullptr)
		{
			inst = new CommandManager();
			inst->commandthread = new std::thread(&CommandManager::starter, inst);
			inst->active = true;
		}
		return inst;
		
	}
	void startcommander();
	void stopcommander();
};


#pragma once
#include <thread>
#include <vector>
#include <iostream>
#include <limits>
#include <fstream>
#include "Keysender.h"
#include "BTService.h"
class CommandManager
{
private:
	CommandManager() {}
	Keysender* keysend = new Keysender();
	int deviceLen = 0;
	DeviceDetails* devices;
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
			inst->active = true;

		}
		return inst;
		
	}
	void loadbtdfile(std::string arg1);
	void loadluafile(std::string arg1);
	void startcommander(bool intro, std::string loadfile = "");
	void stopcommander();
};


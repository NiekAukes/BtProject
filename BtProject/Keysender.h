#pragma once
#include <thread>
#include <vector>
#include <iostream>
#include <string>
#include <sstream>
#include "BTService.h"
#include "LuaFTF.h"
typedef int condition;

struct Rulepart
{
	int id = -1;
	double value = 0.5;
	condition condit;
};

class Rule
{
public:

	int id = -1;
	char key = '\0';
	std::vector<Rulepart> parts;
	/*bool operator==(Rule& other) //compares values and conditions
	{
		//for fingers
		for (int i = 0; i < parts.size(); i++)
		{
			switch (parts[i].condit)
			{
			case 0:
				//equals
				if (!(parts[i].value == other.parts[i].value))
				{
					return false;
				}
				break;

			case 1:
				//greater-or-equal
				if (!(parts[i].value >= other.parts[i].value))
				{
					return false;
				}
				break;

			case 2:
				//greater
				if (!(parts[i].value > other.parts[i].value))
				{
					return false;
				}
				break;

			case 3:
				//less
				if (!(parts[i].value < other.parts[i].value))
				{
					return false;
				}
				break;

			case 4:
				//less-or-equal
				if (!(parts[i].value <= other.parts[i].value))
				{
					return false;
				}
				break;

			case 5:
				//does not equal
				if (!(parts[i].value != other.parts[i].value))
				{
					return false;
				}
				break;
			default:
				return false;
				break;
			}
		}
		return true;
	}*/
};

class Keysender
{
public:
	HANDLE datapipe;
	char databuffer[1024];
	DWORD dwdataread;
	HANDLE errorpipe;
	char errorbuffer[1024];
	DWORD dwerrorread;
	HANDLE inputpipe;
	char inputbuffer[1024];
	DWORD dwinputread;

	HRESULT error = S_OK;

	static std::vector<Rule> ruleset;

	static std::string strcondit[6];
	static bool keysendActive;
	static std::string LuaFile;
	static Keysender* inst;
	
	std::thread* keythread;

	static void Keythreading();
	Keysender();
	void startSender(bool);

};


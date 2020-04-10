#include "LuaFTF.h"
bool LuaFTF::running = true;
BTService* LuaFTF::requestservice;

void LuaFTF::start(std::string filename, BTService* service) {
		requestservice = service;
		std::string cmd = "a = 7 + 11";

		lua_State* L = luaL_newstate();
		luaL_openlibs(L);
		//register functions Kpress, Mpress and recvVar
		lua_register(L, "Kpress", lua_KPress);
		lua_register(L, "Mpress", lua_MPress);
		lua_register(L, "MMove", lua_MMove);
		lua_register(L, "RecvVal", lua_RecvVal);
		lua_register(L, "Exit", lua_Exit);
		lua_register(L, "Sys", lua_Sys);
		if (CheckLua(L, luaL_dofile(L, filename.c_str()))) {
			

			//file loaded, now load Update and loop it. if not found, ignore it


			
		}

		lua_close(L);
	}
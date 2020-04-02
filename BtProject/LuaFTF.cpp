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
		lua_register(L, "RecvVal", lua_RecvVal);
		lua_register(L, "Exit", lua_Exit);
		if (CheckLua(L, luaL_dofile(L, "Main.lua"))) {
			

			//file loaded, now load Update and loop it. if not found, ignore it

			/*if (CheckLua(L, lua_getglobal(L, "Update"))) {
				if (lua_isfunction(L, -1)) {
					while (running) {
						CheckLua(L, lua_pcall(L, 0, 0, 0));
					}
				}
			}*/
			
		}

		lua_close(L);
	}
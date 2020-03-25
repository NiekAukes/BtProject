#pragma once
extern "C" {
#include "Lua535//include/lua.h"
#include "Lua535//include/lauxlib.h"
#include "Lua535//include/lualib.h"
}
#ifdef  _WIN32
#pragma comment(lib, "lua535/liblua53.a")
#endif //  _WIN32

#include <iostream>

class LuaFTF
{
	bool CheckLua(lua_State* L, int r) {
		if (r != LUA_OK) {
			//error
			std::string errormsg = lua_tostring(L, -1);
			std::cout << errormsg << '\n';
			return false;
		}
		return true;
	}
public:
	void start() {
		std::string cmd = "a = 7 + 11";

		lua_State* L = luaL_newstate();
		int r = luaL_dostring(L, cmd.c_str());
		
		if (r == LUA_OK) {
			//proceed
			lua_getglobal(L, "a");
			if (lua_isnumber(L, -1)) {
				float a_in_cpp = (float)lua_tonumber
			}
		}
		else {
			//error
			std::string errormsg = lua_tostring(L, -1);
			std::cout << errormsg << '\n';
		}
		lua_close(L);
	}
};


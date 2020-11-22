#ifndef LUAFTF_H
#define LUAFTF_H
extern "C" {
#include "Lua535//include/lua.h"
#include "Lua535//include/lauxlib.h"
#include "Lua535//include/lualib.h"
}
#ifdef  _WIN32
#pragma comment(lib, "lua535/liblua53.a")
#endif //  _WIN32

#include <iostream>
#include <vector>
#include "BTService.h"
#include <Windows.h>


static bool CheckLua(lua_State* L, int r) {
	if (r != LUA_OK) {
		//error
		std::string errormsg = lua_tostring(L, -1);
		std::cout << errormsg << '\n';
		return false;
	}
	return true;
}



class LuaFTF
{
	
public:
	static BTService* requestservice;
	static bool running;
	void start(std::string filename, BTService* service);
};

template <typename T, typename U>
void as_table(lua_State* L, T begin, U end) {
	lua_newtable(L);
	for (size_t i = 0; begin != end; ++begin, ++i) {
		lua_pushinteger(L, i + 1);
		lua_pushnumber(L, *begin);
		lua_settable(L, -3);
	}
}



static int deb(lua_State* L) {
	int button = lua_tonumber(L, 1);
	int mode = lua_tonumber(L, 2);

	INPUT ip;
	ip.type = INPUT_MOUSE;
	ip.mi.dx = 0; ip.mi.dy = 0;
	ip.mi.time = 0;
	ip.mi.dwExtraInfo = 0;
	ip.mi.mouseData = 0;
	ip.mi.dwFlags = button;
	UINT ret = SendInput(1, &ip, sizeof(INPUT));
	return 0;
}

static int lua_RecvVal(lua_State* L/*, short* val*/) {
	float* values = nullptr;
	//double value[4] = { 4.0, 11.0, 1.0, 1.5 };
	int amount = 0;
	values = LuaFTF::requestservice->getDoubleDataFromBT(&amount);

	const size_t s = amount;
	//std::vector<float> vec(values, values + amount);

	//as_table(L, vec.begin(), vec.end());
	lua_newtable(L);
	for (int i = 0; i < amount; i++) {
		lua_pushinteger(L, i + 1);
		lua_pushnumber(L, *(values + i));
		lua_settable(L, -3);
	}
	return 1;
}




static int lua_MMove(lua_State* L) {
	int dx = lua_tonumber(L, 1);
	int dy = lua_tonumber(L, 2);
	int mode = lua_tonumber(L, 3);

	INPUT ip;
	ip.type = INPUT_MOUSE;
	ip.mi.dx = dx; ip.mi.dy = dy;
	ip.mi.time = 0;
	ip.mi.dwExtraInfo = 0;
	ip.mi.mouseData = 0;
	if (mode)
		ip.mi.dwFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_VIRTUALDESK | MOUSEEVENTF_ABSOLUTE;
	else
		ip.mi.dwFlags = MOUSEEVENTF_MOVE;
	UINT ret = SendInput(1, &ip, sizeof(INPUT));
	return 0;
}
static int lua_KPress(lua_State* L/*, char Key, int mode*/) {
	size_t p = 0;
	char Key = *lua_tolstring(L, 1, &p);
	int mode = (char)lua_tonumber(L, 2);

	INPUT ip;
	ip.type = INPUT_KEYBOARD;
	ip.ki.wScan = 0;
	ip.ki.time = 0;
	ip.ki.dwExtraInfo = 0;
	if (mode)
		ip.ki.dwFlags = KEYEVENTF_KEYUP;
	else
		ip.ki.dwFlags = 0; 

	ip.ki.wVk = toupper(Key);
	SendInput(1, &ip, sizeof(INPUT));
	return 0;
}
static int lua_MPress(lua_State* L/*, int Button, int mode*/) {
	deb(L);
	return 0;
}

static int lua_Sys(lua_State* L) {
	const char* comm = lua_tostring(L, 1);
	system(comm);
	return 1;
}
static int lua_Exit(lua_State* L) {
	LuaFTF::running = false;
	return 0;
}
#endif
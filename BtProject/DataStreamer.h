#pragma once
#define BUFFERSIZE 1024

#include <thread>
#include <vector>
#include <iostream>
#include <string>
#include <sstream>
#include <Windows.h>
namespace dts {
	static class DataStreamer
	{
	};

	class dataStream {
	private:
		HANDLE datapipe;
		char databuffer[BUFFERSIZE];
		DWORD dwdataread;

		HRESULT error = S_OK;

	public:
		dataStream(char* name);

		void operator<<(char* dat);
		void operator<<(wchar_t* dat);
		void operator<<(long long int dat);
		void operator<<(int dat);
		void operator<<(bool dat);
		void operator<<(double dat);
		void operator<<(float dat);
		void operator<<(std::string dat);
	};
}
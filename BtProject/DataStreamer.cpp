#include "DataStreamer.h"

dts::dataStream::dataStream(char* name)
{
}

void dts::dataStream::operator<<(char* dat)
{

	if (datapipe != INVALID_HANDLE_VALUE) {
		if (ConnectNamedPipe(datapipe, NULL) != FALSE) {
			WriteFile(datapipe, dat, strlen(dat), &dwdataread, NULL);
		}
	}
}

void dts::dataStream::operator<<(wchar_t* dat)
{
	if (datapipe != INVALID_HANDLE_VALUE) {
		if (ConnectNamedPipe(datapipe, NULL) != FALSE) {
			WriteFile(datapipe, dat, wcslen(dat), &dwdataread, NULL);
		}
	}
}

void dts::dataStream::operator<<(long long int dat)
{
	if (datapipe != INVALID_HANDLE_VALUE) {
		if (ConnectNamedPipe(datapipe, NULL) != FALSE) {
			WriteFile(datapipe, &dat, 16, &dwdataread, NULL);
		}
	}
}

void dts::dataStream::operator<<(int dat)
{
	if (datapipe != INVALID_HANDLE_VALUE) {
		if (ConnectNamedPipe(datapipe, NULL) != FALSE) {
			WriteFile(datapipe, &dat, 4, &dwdataread, NULL);
		}
	}
}

void dts::dataStream::operator<<(bool dat)
{
	if (datapipe != INVALID_HANDLE_VALUE) {
		if (ConnectNamedPipe(datapipe, NULL) != FALSE) {
			WriteFile(datapipe, &dat, 1, &dwdataread, NULL);
		}
	}
}

void dts::dataStream::operator<<(double dat)
{
	if (datapipe != INVALID_HANDLE_VALUE) {
		if (ConnectNamedPipe(datapipe, NULL) != FALSE) {
			WriteFile(datapipe, &dat, 8, &dwdataread, NULL);
		}
	}
}

void dts::dataStream::operator<<(float dat)
{
	if (datapipe != INVALID_HANDLE_VALUE) {
		if (ConnectNamedPipe(datapipe, NULL) != FALSE) {
			WriteFile(datapipe, &dat, 4, &dwdataread, NULL);
		}
	}
}

void dts::dataStream::operator<<(std::string dat)
{
	if (datapipe != INVALID_HANDLE_VALUE) {
		if (ConnectNamedPipe(datapipe, NULL) != FALSE) {
			WriteFile(datapipe, dat.c_str(), dat.size(), &dwdataread, NULL);
		}
	}
}

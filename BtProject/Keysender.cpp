#include "Keysender.h"

std::string Keysender::strcondit[] = { "==", ">=", ">", "<", "<=", "!=" };
bool Keysender::keysendActive = false;
void Keysender::Keythreading()
{
	while (keysendActive)
	{

	}
}

Keysender::Keysender()
{
	keysendActive = true;
	keythread = new std::thread(Keythreading);
}

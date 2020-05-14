// ConsoleApplication1.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <string>
#include <thread>

bool waiting = false;
void s(std::string* cmd) {
	std::string s;
	std::getline(std::cin, s);

	while (!waiting) {}
	s.push_back('\n');
	cmd->append(s);
}
int main()
{
	std::string ma = "a;kljdf;alf";
    std::cout << "Hello World!\n";
	std::thread th(s, &ma);
	while (1) {
		while (ma.find('\n') == std::string::npos) { waiting = true; }
		std::cout << ma;
		waiting = false;
		ma = "";
	}
}



// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file

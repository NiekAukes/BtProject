#pragma once
#include <iostream>
#include <type_traits>
#include <string>
#include <sstream>
#include <vector>
//enum Logictype {
//	And,
//	Or,
//	Nand,
//	Nor,
//	Xor,
//	Xnor
//};
enum Expressiontype {
	Equal,
	NotEqual,
	Greater,
	GreaterOrEqual,
	Less,
	LessOrEqual,
	And = 0,
	Or = 1,
	Nand = 2,
	Nor = 3,
	Xor = 4,
	Xnor = 5
};
class Logic
{
public:
	virtual bool Value() = 0;
	static Logic* strToLogic(std::string strin);
};

//class BiValue : public Logic {
//public:
//	BiValue() {}
//	BiValue(Logic* v1, Logic* v2) : val1(v1), val2(v2) {
//
//	}
//	BiValue(Logic* v1, Logic* v2, Logictype2 Operation) : val1(v1), val2(v2), type(Operation) {
//
//	}
//	Logictype2 type = Or;
//	Logic* val1 = nullptr;
//	Logic* val2 = nullptr;
//	bool Value();
//};

template <class T>
class Expression : public Logic {
public:
	Expression() {}
	Expression(T* v1, T* v2) : val1(v1), val2(v2) {

	}
	Expression(T* v1, T* v2, Expressiontype Operation) : val1(v1), val2(v2), type(Operation) {

	}
	Expressiontype type = Equal;
	T* val1;
	T* val2;
	void value() {
		Logic* l1 = dynamic_cast<Logic*>(val1);
		Logic* l2 = dynamic_cast<Logic*>(val2);
		bool v = false;
		switch (type)
		{
		case And:
			v = (l1->Value() && l2->Value());
			return v;
			break;
		case Or:
			return (l1->Value() || l2->Value());
			break;
		case Nand:
			return !(l1->Value() && l2->Value());
			break;
		case Nor:
			return !(l1->Value() || l2->Value());
			break;
		case Xor:
			return (!(l1->Value() && l2->Value()) && (l1->Value() || l2->Value()));
			break;
		case Xnor:
			return !(!(l1->Value() && l2->Value()) && (l1->Value() || l2->Value()));
			break;
		default:
			return false;
			break;
		}
	}

};
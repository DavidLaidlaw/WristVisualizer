#pragma once

namespace libCoin3D {
public ref class Coin3DBase
{
public:
	static void Init();
	static void Init(System::String^ appname);
private:
	static bool _initialized = false;
};
}

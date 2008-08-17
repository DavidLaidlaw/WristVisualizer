#include "StdAfx.h"
#include "Coin3DBase.h"

#include <Inventor/Win/SoWin.h>


void libCoin3D::Coin3DBase::Init()
{
	if (_initialized) //check if already done 
		return;
	SoWin::init("");

	_initialized = true;
}

void libCoin3D::Coin3DBase::Init(System::String^ appname)
{
	if (_initialized) //check if already done 
		return;

	char* name = (char*)System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(appname).ToPointer();
	SoWin::init(name);
	System::Runtime::InteropServices::Marshal::FreeHGlobal((System::IntPtr)name);

	_initialized = true;
}

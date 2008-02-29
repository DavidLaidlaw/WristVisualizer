#include "StdAfx.h"
#include "Label2D.h"
#include <Inventor/nodes/SoSeparator.h>
#include <Inventor/nodes/SoCone.h>
#include <Inventor/nodes/SoTransform.h>

libCoin3D::Label2D::Label2D()
{
	_node = new SoSeparator();
	_node->ref();
	_translation = new SoTranslation();
	_text = new SoText2;
	_translation->translation.setValue(-0.90f, 0.90f, 0.);
	_font = new SoFont;
	_font->size.setValue(200);

	
	_node->addChild(_translation);
	_node->addChild(_font);
	_node->addChild(_text);
}

libCoin3D::Label2D::~Label2D()
{
	_node->unref();
}

void libCoin3D::Label2D::setFontSize(int size)
{
	_font->size.setValue(200);
}

void libCoin3D::Label2D::setLocation(float x, float y) 
{
	_translation->translation.setValue(x,y,0);
}

void libCoin3D::Label2D::setText(System::String ^text) 
{
	char* label = (char*)System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(text).ToPointer();
	_text->string.setValue(label);
	System::Runtime::InteropServices::Marshal::FreeHGlobal((System::IntPtr)label);
}
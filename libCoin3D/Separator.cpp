#include "StdAfx.h"
#include "Separator.h"

libCoin3D::Separator::Separator(void)
{
	_separator = new SoSeparator();
	_style = NULL;
}


void libCoin3D::Separator::addChild(Separator^ child)
{
	_separator->addChild(child->getSoSeparator());
}


void libCoin3D::Separator::addFile(System::String^ filename)
{
	addFile(filename,false);
}
void libCoin3D::Separator::addFile(System::String^ filename, bool canhide)
{
	
	char* test = (char*)System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(filename).ToPointer();
	_style = new SoDrawStyle;

	SoInput in;
	if ( in.openFile(test) ) {
		SoSeparator *bone = SoDB::readAll( &in );
		if (bone==NULL) //small error checking
			throw gcnew System::ArgumentException("Error parsing IV file: "+filename);
		_separator->addChild(_style);
		_separator->addChild(bone);
	}
	//System::Runtime::InteropServices::Marshal::FreeHGlobal(test);
}

void libCoin3D::Separator::addNode(Node^ node)
{
	_separator->addChild(node->getNode());
}

void libCoin3D::Separator::removeChild(Separator^ child)
{
	_separator->removeChild(child->getSoSeparator());
}

void libCoin3D::Separator::hide()
{
	if (_style != NULL) 
		_style->style = SoDrawStyle::INVISIBLE;
}

void libCoin3D::Separator::show()
{
	if (_style != NULL) 
		_style->style = SoDrawStyle::FILLED;
}


SoSeparator* libCoin3D::Separator::getSoSeparator(void)
{
	return _separator;
}

void libCoin3D::Separator::addTransform(libCoin3D::Transform ^transform)
{
	_separator->insertChild(transform->getSoTransform(),0);
	_numTransforms++;
}

void libCoin3D::Separator::removeTransform()
{
	if (_numTransforms>0) {
		_separator->removeChild(0);
		_numTransforms--;
	}
}

SoNode* libCoin3D::Separator::getNode()
{
	return _separator;
}

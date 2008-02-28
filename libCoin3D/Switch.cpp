#include "StdAfx.h"
#include "Switch.h"

libCoin3D::Switch::Switch()
{
	_switch = new SoSwitch();
}


SoNode* libCoin3D::Switch::getNode()
{
	return _switch;
}

void libCoin3D::Switch::addChild(libCoin3D::Node ^node) 
{
	_switch->addChild(node->getNode());
}

void libCoin3D::Switch::whichChild(int childIndex) 
{
	_switch->whichChild.setValue(childIndex);
}
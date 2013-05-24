#include "StdAfx.h"
#include "Switch.h"

#include <Inventor/nodes/SoSeparator.h>

libCoin3D::Switch::Switch()
{
	_switch = new SoSwitch();
	_switch->ref();
}

libCoin3D::Switch::!Switch()
{
	_switch->unref();
}


SoNode* libCoin3D::Switch::getNode()
{
	return _switch;
}

void libCoin3D::Switch::reference()
{
	_switch->ref();
}

void libCoin3D::Switch::unref()
{
	_switch->unref();
}

void libCoin3D::Switch::unrefNoDelete()
{
	_switch->unrefNoDelete();
}

void libCoin3D::Switch::addChild(libCoin3D::Node ^node) 
{
	if (node->getNode() != NULL) //check to make sure we have a valid node
		_switch->addChild(node->getNode());
	else //if there is no child, then we put in a placeholder
		_switch->addChild(new SoSeparator);
}

void libCoin3D::Switch::whichChild(int childIndex) 
{
	_switch->whichChild.setValue(childIndex);
}

void libCoin3D::Switch::hideAll()
{
	_switch->whichChild.setValue(SO_SWITCH_NONE);
}
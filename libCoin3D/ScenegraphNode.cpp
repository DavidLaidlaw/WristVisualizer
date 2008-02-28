#include "StdAfx.h"
#include "ScenegraphNode.h"

#include <Inventor/nodes/SoGroup.h>

libCoin3D::ScenegraphNode::ScenegraphNode()
{
}

libCoin3D::ScenegraphNode::ScenegraphNode(SoNode* node)
{
	_node = node;
	buildLocalData();
}

libCoin3D::ScenegraphNode::ScenegraphNode(libCoin3D::Separator^ separator)
{
	_node = separator->getSoSeparator();
	buildLocalData();
}

array<libCoin3D::ScenegraphNode^>^ libCoin3D::ScenegraphNode::getChildren()
{
	return _children;
}

void libCoin3D::ScenegraphNode::buildLocalData()
{
	if (_node==nullptr) return;
	_name = gcnew System::String(_node->getName().getString());
	_typeName = gcnew System::String(_node->getTypeId().getName().getString());

	//check if not a group node
	if (!_node->isOfType(SoGroup::getClassTypeId())) {
		_children = gcnew array<ScenegraphNode^>(0);
		return;
	}

	//else we are a group
	SoGroup* myGroup = (SoGroup*)_node;
	_children = gcnew array<ScenegraphNode^>(myGroup->getNumChildren());	
	for (int i=0; i<myGroup->getNumChildren(); i++) {
		_children[i] = gcnew ScenegraphNode(myGroup->getChild(i));
	}

}

SoNode* libCoin3D::ScenegraphNode::getNode()
{
	return _node;
}
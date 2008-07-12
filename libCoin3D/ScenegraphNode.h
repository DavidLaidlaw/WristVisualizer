#pragma once

#include <Inventor/nodes/SoGroup.h>
#include "Node.h"
#include "Separator.h"

namespace libCoin3D {
public ref class ScenegraphNode : Node
{
public:
	ScenegraphNode();
	ScenegraphNode(SoNode* node);
	ScenegraphNode(Separator^ separator);

	array<ScenegraphNode^>^ getChildren();

	//Property accessors
	property System::String^ Name  { System::String^ get() { return _name; } }
	property System::String^ TypeName  { System::String^ get() { return _typeName; } }

	virtual bool isEqualSeparator(Separator^ separator);
	virtual Separator^ ToSeparator();

	virtual SoNode* getNode() override;

private:
	void buildLocalData();
	System::String^ _name;
	System::String^ _typeName;

	SoNode* _node;
	array<ScenegraphNode^>^ _children;
};
}

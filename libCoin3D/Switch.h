#pragma once

#include "Node.h"
#include <Inventor/nodes/SoNode.h>
#include <Inventor/nodes/SoSwitch.h>

namespace libCoin3D {
public ref class Switch : Node
{
public:
	Switch();
	virtual SoNode* getNode() override;
	void addChild(Node^ node);
	void whichChild(int childIndex);
private: 
	SoSwitch* _switch;
};
}
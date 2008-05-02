#pragma once

#include "Node.h"
#include <Inventor/nodes/SoNode.h>
#include <Inventor/nodes/SoSwitch.h>

namespace libCoin3D {
public ref class Switch : Node
{
public:
	Switch();
	!Switch();
	virtual SoNode* getNode() override;
	void addChild(Node^ node);
	void whichChild(int childIndex);
	void hideAll();
	virtual void reference();
	virtual void unref();
	virtual void unrefNoDelete();
private: 
	SoSwitch* _switch;
};
}
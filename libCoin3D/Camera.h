#pragma once

#include "Node.h"
#include <Inventor/nodes/SoSeparator.h>
#include <Inventor/nodes/SoOrthographicCamera.h>


namespace libCoin3D {
public ref class Camera : Node
{
public:
	Camera();
	virtual SoNode* getNode() override { return _node; }
private:
	SoNode* _node;
};
}

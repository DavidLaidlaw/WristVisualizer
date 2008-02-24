#pragma once

#include "Node.h"
#include <Inventor/nodes/SoNode.h>
#include <Inventor/nodes/SoSphere.h>
#include <Inventor/nodes/SoSeparator.h>

namespace libCoin3D {
public ref class Sphere : Node
{
public:
	Sphere();
	Sphere(float radius);
	virtual SoNode* getNode() override;
private:
	SoSphere* _sphere;
};
}

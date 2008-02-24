#pragma once

#include "Node.h"
#include <Inventor/nodes/SoNode.h>
#include <Inventor/nodes/SoMaterial.h>

namespace libCoin3D {
public ref class Material : Node
{
public:
	Material();
	virtual void setColor(float r, float g, float b);
	virtual void setTransparency(float transparency);
	virtual SoNode* getNode() override;
private: 
	SoMaterial* _material;
};
}
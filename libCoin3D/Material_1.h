#pragma once

#include "Node.h"
#include <Inventor/nodes/SoNode.h>
#include <Inventor/nodes/SoMaterial.h>

namespace libCoin3D {
public ref class Material : Node
{
public:
	Material();
	~Material();
	Material(SoMaterial* material);
	virtual void setColor(float r, float g, float b);
	virtual void setColor(System::Drawing::Color color);
	virtual void setTransparency(float transparency);
	virtual float getTransparency();
	virtual int getPackedColor();
	virtual System::Drawing::Color getColor();
	static System::Drawing::Color GetDefaultColor();
	virtual void setOverride(bool forceOverride);
	virtual SoNode* getNode() override;
private: 
	SoMaterial* _material;
};
}
#pragma once
#include "node.h"

#include <Inventor/nodes/SoScale.h>


namespace libCoin3D {
public ref class Scale : Node
{
public:
	Scale();
	void reference();
	void setScaleFactor(float x, float y, float z);
	virtual SoNode* getNode() override;
	//System::String^ getNodeGraph();

private:
	SoScale* _scale;
};


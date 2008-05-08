#pragma once

#include "Node.h"
#include <Inventor/nodes/SoSeparator.h>
#include <Inventor/nodes/SoLineSet.h>
#include <Inventor/nodes/SoCoordinate3.h>

namespace libCoin3D {
public ref class Contour : Node
{
public:
	Contour();
	virtual void addLineSegment(float x0, float y0, float z0, float x1, float y1, float z1);

	virtual SoNode* getNode() override { return _node; }
private:
	SoSeparator* _node;
	SoCoordinate3* _pts;
	SoLineSet* _lineSet;
};
}
#pragma once

#include "Node.h"
#include "Separator.h"

namespace libCoin3D {
public ref class ColoredBone : Node
{
public:
	ColoredBone(System::String^ filename);
	virtual int getNumberVertices() { return _numColoredVertices; }
	virtual void setColorMap(array<int>^ colors);
	virtual void setHidden(bool hidden);

	virtual SoNode* getNode() override { return _node; }

	
private:
	SoSeparator* _node;
	SoVertexProperty* _vertexProperty;
	SoDrawStyle* _drawstyle;
	int _numColoredVertices;
};
}
#pragma once

#include "Node.h"
#include "Separator.h"

namespace libCoin3D {
public ref class ColoredBone : Node
{
public:
	ColoredBone(System::String^ filename);
	!ColoredBone();
	virtual int getNumberVertices() { return _numColoredVertices; }
	virtual void setupFullColorMap(array<array<int>^>^ colors);
	virtual void setColorMap(array<int>^ colors);
	virtual void setColorIndex(int index);

	virtual void setHidden(bool hidden);

	virtual SoNode* getNode() override { return _node; }

	virtual array<float,2>^ getVertices();

	
private:
	SoSeparator* _node;
	SoVertexProperty* _vertexProperty;
	SoDrawStyle* _drawstyle;
	int _numColoredVertices;
	int _numPositions; 

	unsigned int* _colorData;
	unsigned int** _fullColormap;
};
}
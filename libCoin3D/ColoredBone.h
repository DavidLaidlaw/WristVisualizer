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
	virtual array<int,2>^ getFaceSetIndices();

	virtual void clearColorMap();

	
private:
	SoSeparator* _node;
	SoVertexProperty* _vertexProperty;
	SoIndexedFaceSet* _indexedFaceSet;
	SoDrawStyle* _drawstyle;
	int _numColoredVertices;
	int _numPositions; 

	unsigned int* _colorData;
	unsigned int** _fullColormap;
};
}
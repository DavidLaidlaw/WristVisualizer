#pragma once

#include "Node.h"
#include "Separator.h"
#include "Contour.h"

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
	virtual void clearColorMap();

	virtual void setVisibility(bool visible);
	virtual void addNode(Node^ node);
	virtual void removeChild(Node^ node);
	virtual void insertNode(Node^ node, int position);
	virtual void addContour(Contour^ contour);
	virtual void setAndReplaceContour(Contour^ contour);
	virtual void removeContour();

	virtual SoNode* getNode() override { return _node; }

	virtual array<float,2>^ getVertices();
	virtual array<int,2>^ getFaceSetIndices();
	
private:
	SoSeparator* _node;
	SoVertexProperty* _vertexProperty;
	SoIndexedFaceSet* _indexedFaceSet;
	SoDrawStyle* _drawstyle;
	int _numColoredVertices;
	int _numPositions; 
	bool _visible;

	Contour^ _contour;

	unsigned int* _colorData;
	unsigned int** _fullColormap;

	array<float,2>^ _vertices;
	array<int,2>^ _faceSet;
};
}
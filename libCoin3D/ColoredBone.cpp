#include "StdAfx.h"
#include "ColoredBone.h"

#include <Inventor/nodes/SoVertexProperty.h>
#include <Inventor/nodes/SoTransform.h>
//#include <Inventor/nodes/SoNormal.h>
#include <Inventor/nodes/SoMaterial.h>
#include <Inventor/nodes/SoMaterialBinding.h>
#include <Inventor/nodes/SoDrawStyle.h>
#include <Inventor/nodes/SoShapeHints.h>

libCoin3D::ColoredBone::ColoredBone(System::String^ filename)
{
	_node = new SoSeparator();
	_node->ref();
	char* charFilename = (char*)System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(filename).ToPointer();
	SoSeparator *bone;
	SoInput in;
	if ( in.openFile(charFilename) ) {
		bone = SoDB::readAll( &in );
		if (bone==NULL) //small error checking
			throw gcnew System::ArgumentException("Error parsing IV file: "+filename);
	}
	else
		throw gcnew System::ArgumentException("Error opening IV file: "+filename);

	bone->ref();

	//TODO: Save name somewhere... in _node
	System::Runtime::InteropServices::Marshal::FreeHGlobal((System::IntPtr)charFilename);

	//now lets try and find the parts that we need
	SoCoordinate3* pCoordinate3 = NULL;
	SoTransform* pTransform = NULL;
    SoIndexedFaceSet* pBoneIndexedFaceSet = NULL;

	int numChildren = bone->getNumChildren();
	SoSeparator* innerBone = bone;
	if (numChildren==1 && bone->getChild(0)->getTypeId()==SoSeparator::getClassTypeId()) {
		innerBone = (SoSeparator*)bone->getChild(0);
		numChildren = innerBone->getNumChildren();
	}
	for (int i=0; i < numChildren; i++) {
		if (innerBone->getChild(i)->getTypeId()==SoCoordinate3::getClassTypeId())
		{
			pCoordinate3 = (SoCoordinate3*)innerBone->getChild(i);
		}
		else if (innerBone->getChild(i)->getTypeId()==SoIndexedFaceSet::getClassTypeId())
		{
			pBoneIndexedFaceSet = (SoIndexedFaceSet*)innerBone->getChild(i);
		}
		else if (innerBone->getChild(i)->getTypeId()==SoTransform::getClassTypeId())
		{
			pTransform = (SoTransform*)innerBone->getChild(i);
		}
	}

	if (pCoordinate3==NULL || pBoneIndexedFaceSet==NULL)
		throw gcnew System::ArgumentException("Error parsing file: "+filename + ", not enough information to build the model");

	SoVertexProperty* pBoneVertexProperty = new SoVertexProperty;
	pBoneVertexProperty->materialBinding = SoMaterialBinding::PER_VERTEX_INDEXED;
	pBoneVertexProperty->vertex = pCoordinate3->point;
	pBoneIndexedFaceSet->vertexProperty.setValue(pBoneVertexProperty);

	_numColoredVertices = pBoneVertexProperty->vertex.getNum();
	SoShapeHints *shapehints = new SoShapeHints;
    shapehints->vertexOrdering = SoShapeHints::COUNTERCLOCKWISE;
    shapehints->shapeType = SoShapeHints::SOLID;

    //drawing style;
    _drawstyle = new SoDrawStyle;

	if (pTransform != NULL)
		_node->addChild(pTransform);

    //build scene graph
    _node->addChild(shapehints);
    _node->addChild(_drawstyle);

    _node->addChild(pBoneIndexedFaceSet);
    _node->unrefNoDelete();

	_vertexProperty = pBoneVertexProperty;
	_indexedFaceSet = pBoneIndexedFaceSet;

    //delete file contents
    bone->unref();

	_fullColormap = NULL;
	_colorData = new unsigned int[_numColoredVertices]; 
}

libCoin3D::ColoredBone::!ColoredBone()
{
	if (_fullColormap != NULL) {
		for (int i=0; i< _numPositions; i++)
			if (_fullColormap[i] != NULL)
				delete _fullColormap[i];
		delete _fullColormap;
	}
	if (_colorData != NULL)
		delete _colorData;
}

void libCoin3D::ColoredBone::setColorMap(array<int>^ colors)
{
	if (colors->Length != _numColoredVertices)
		throw gcnew System::ArgumentException("length of color array does not match the number of vertices");

	//unsigned int* colorData = new unsigned int[_numColoredVertices]; 
	System::Runtime::InteropServices::Marshal::Copy(colors,0,(System::IntPtr)_colorData,_numColoredVertices);
	_vertexProperty->orderedRGBA.setValues(0,_numColoredVertices,_colorData);
}

void libCoin3D::ColoredBone::setColorIndex(int index)
{
	_vertexProperty->orderedRGBA.setValues(0,_numColoredVertices,_fullColormap[index]);
}

void libCoin3D::ColoredBone::clearColorMap()
{
	_vertexProperty->orderedRGBA.deleteValues(0);
}

void libCoin3D::ColoredBone::setupFullColorMap(array<array<int>^>^ colors)
{
	_fullColormap = new unsigned int*[colors->Length];
	_numPositions = colors->Length;
	for (int i=0; i<colors->Length; i++) {
		_fullColormap[i] = new unsigned int[_numColoredVertices];
		System::Runtime::InteropServices::Marshal::Copy(colors[i],0,(System::IntPtr)_fullColormap[i],_numColoredVertices);
	}
}

void libCoin3D::ColoredBone::setHidden(bool hidden)
{
	if (hidden) {
		_drawstyle->style = SoDrawStyle::INVISIBLE;
	}
	else {
		_drawstyle->style = SoDrawStyle::FILLED;
	}

	//now the contour, if it exists
	if (_contour != nullptr) _contour->setHidden(hidden);
}

void libCoin3D::ColoredBone::addNode(Node^ node)
{
	if (node != nullptr && node->getNode() != NULL)
		_node->addChild(node->getNode());
}

void libCoin3D::ColoredBone::insertNode(Node^ node, int position)
{
	_node->insertChild(node->getNode(), position);
}

void libCoin3D::ColoredBone::removeChild(Node^ child)
{
	_node->removeChild(child->getNode());
}

void libCoin3D::ColoredBone::addContour(Contour^ contour)
{
	_contour = contour;
	_node->addChild(contour->getNode());
}

array<float,2>^ libCoin3D::ColoredBone::getVertices()
{
	if (_vertices==nullptr) {
		int numPts = _vertexProperty->vertex.getNum();
		_vertices = gcnew array<float,2>(numPts,3);
		for (int i=0; i<numPts; i++) {
			_vertices[i,0] = _vertexProperty->vertex[i][0];
			_vertices[i,1] = _vertexProperty->vertex[i][1];
			_vertices[i,2] = _vertexProperty->vertex[i][2];
		}
	}
	return _vertices;
}

array<int,2>^ libCoin3D::ColoredBone::getFaceSetIndices()
{
	if (_faceSet==nullptr) {
		int numTriangles = _indexedFaceSet->coordIndex.getNum();
		numTriangles = numTriangles/4;
		_faceSet = gcnew array<int,2>(numTriangles,3);  //yes, only copying the indices, not the -1
		for (int i=0; i<numTriangles; i++) {
			_faceSet[i,0] = _indexedFaceSet->coordIndex[i*4];
			_faceSet[i,1] = _indexedFaceSet->coordIndex[i*4+1];
			_faceSet[i,2] = _indexedFaceSet->coordIndex[i*4+2];
		}
	}
	return _faceSet;
}
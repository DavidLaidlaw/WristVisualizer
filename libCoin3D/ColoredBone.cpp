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
	for (int i=0; i < numChildren; i++) {
		if (bone->getChild(i)->getTypeId()==SoCoordinate3::getClassTypeId())
		{
			pCoordinate3 = (SoCoordinate3*)bone->getChild(i);
		}
		else if (bone->getChild(i)->getTypeId()==SoIndexedFaceSet::getClassTypeId())
		{
			pBoneIndexedFaceSet = (SoIndexedFaceSet*)bone->getChild(i);
		}
		else if (bone->getChild(i)->getTypeId()==SoTransform::getClassTypeId())
		{
			pTransform = (SoTransform*)bone->getChild(i);
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

    //delete file contents
    bone->unref();
}

void libCoin3D::ColoredBone::setColorMap(array<int>^ colors)
{
	if (colors->Length != _numColoredVertices)
		throw gcnew System::ArgumentException("length of color array does not match the number of vertices");

	unsigned int* colorData = new unsigned int[_numColoredVertices]; //TODO: Fix memory leak :)
	System::Runtime::InteropServices::Marshal::Copy(colors,0,(System::IntPtr)colorData,_numColoredVertices);
	_vertexProperty->orderedRGBA.setValues(0,_numColoredVertices,colorData);
}

void libCoin3D::ColoredBone::setHidden(bool hidden)
{
	if (hidden) {
		_drawstyle->style = SoDrawStyle::INVISIBLE;
	}
	else {
		_drawstyle->style = SoDrawStyle::FILLED;
	}
}
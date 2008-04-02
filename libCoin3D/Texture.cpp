#include "StdAfx.h"
#include "Texture.h"

#include <Inventor/nodes/SoSeparator.h>
#include <Inventor/nodes/SoMaterial.h>
#include <Inventor/nodes/SoCoordinate3.h>
#include <Inventor/nodes/SoPointSet.h>
#include <Inventor/nodes/SoIndexedLineSet.h>
#include <Inventor/nodes/SoTexture2.h>
#include <Inventor/draggers/SoTranslate1Dragger.h>
#include <Inventor/nodes/SoTransform.h>
#include <Inventor/fields/SoSFInt32.h>
#include <Inventor/SoDB.h>
#include <Inventor/nodes/SoTextureCoordinate2.h>
#include <Inventor/nodes/SoTextureCoordinateBinding.h>
#include <Inventor/engines/SoCompose.h>
#include <Inventor/engines/SoCalculator.h>
#include <Inventor/nodes/SoTranslation.h>
#include <Inventor/nodes/SoFaceSet.h>
#include <Inventor/nodes/SoBaseColor.h>
#include <Inventor/nodes/SoDrawStyle.h>


libCoin3D::Texture::Texture(void)
{
	_sizeX = 512;
	_sizeY = 512;
	_sizeZ = 187;
	_voxelX = 0.273438;
	_voxelY = 0.273438;
	_voxelZ = 0.625000;
	//allocate data
	//_all_slice_data1 = allocateSliceStack(_sizeX,_sizeY,_sizeZ);
	_current_slice1 = new unsigned char[_sizeX*_sizeY];
	_verticesRectangle1 = NULL;

}

libCoin3D::Texture::~Texture()
{
	if (_all_slice_data1 != NULL) {
		for (int i=0; i<_sizeZ; i++)
			delete _all_slice_data1[i];
		delete _all_slice_data1;
	}

	if (_current_slice1 != NULL)
		delete _current_slice1;

	if (_verticesRectangle1 != NULL) {
		for (int i=0; i<4; i++)
			delete _verticesRectangle1[i];
		delete _verticesRectangle1;
	}
}

unsigned char** libCoin3D::Texture::allocateSliceStack(int numPixelsX, int numPixelsY, int numPixelsZ)
{
	unsigned char** allStacks = new unsigned char*[numPixelsZ];
	for (int i=0; i<numPixelsZ; i++) {
		allStacks[i] = new unsigned char[numPixelsX*numPixelsZ];
	}
	return allStacks;
}

libCoin3D::Separator^ libCoin3D::Texture::createPointsFileObject(cli::array<cli::array<double >^,1> ^points)
{
	SoSeparator* bone = new SoSeparator();
	bone->ref();

	SoBaseColor* clr = new SoBaseColor();
	SoDrawStyle* drawStyle = new SoDrawStyle();
	bone->addChild(drawStyle);
	bone->addChild(clr);
	
	drawStyle->style = SoDrawStyle::POINTS;
	drawStyle->pointSize = 1;
	drawStyle->setOverride(TRUE);

	clr->rgb.setValue(SbColor(1.0f, 0.0f, 0.1f));

	SoCoordinate3* coord = new SoCoordinate3;
	SoPointSet* pts = new SoPointSet;

	SbVec3f pt;
	SoVertexProperty *lineVtxProp= new SoVertexProperty();
	SoIndexedLineSet *line= new SoIndexedLineSet();
	line->vertexProperty.setValue(lineVtxProp);

	for (int i=0; i<points->Length; i++) {
		pt = SbVec3f((float)points[i][0], (float)points[i][1], (float)points[i][2]);
		lineVtxProp->vertex.set1Value(i,pt);
		line->coordIndex.set1Value(i,i);
		coord->point.set1Value(i,pt);
	}
	line->coordIndex.set1Value(points->Length, SO_END_LINE_INDEX);
	bone->addChild(coord);
	bone->addChild(pts);
	bone->unrefNoDelete();

	return gcnew Separator(bone);
}

struct TextureCBData {
  int axis;
  SoTexture2 * texture; 
  unsigned char** buffer;  
  int sizeX;
  int sizeY;
  int sizeZ;
};

void updateTextureCB( void * data, SoSensor * )
{
	int xf;
	TextureCBData * textureCBdata  = (TextureCBData *) data;  

	SoTexture2  * texture = textureCBdata->texture;
	unsigned char** buffer = textureCBdata->buffer;

	int axis = textureCBdata -> axis;
	if ( texture == 0 ) {
		return;
	}

	// Make the counter change the vtxProperty RGB colours
	// Find out which transformation we should display colours for
	//if (axis == 2) 
		xf = ((SoSFInt32*) (SoDB::getGlobalField("test1")))->getValue();
	//else xf = ((SoSFInt32*) (SoDB::getGlobalField("offbeX")))->getValue();
	switch( axis )
	{
	case 2:
		//init_tmp_buf( tmp_buf, buffer[xf]);
		texture -> image.setValue(SbVec2s(textureCBdata->sizeX, textureCBdata->sizeY),1, (const unsigned char*) buffer[xf] );   

		break;
	case 0:
		//init_tmp_buf_x( tmp_buf_x, buffer[xf]);		
		//texture -> image.setValue(SbVec2s(MRI_Y_SIZE_vert, MRI_Z_SIZE_vert),1, (const unsigned char*) tmp_buf_x );   
		break;
	}
}



void libCoin3D::Texture::setTextureZplane(SoTexture2* texture, unsigned char** all_slice_data)
{
	/*
	init_tmp_buf( tmp_buf, all_slice_data[0]);
    if(MRI_Y_SIZE_vert > MRI_Y_SIZE)
		memset(tmp_bufThird, MRI_X_SIZE*MRI_Y_SIZE*sizeof(unsigned char),0);

	texture->image.setValue(SbVec2s(MRI_X_SIZE, MRI_Y_SIZE),1, (const unsigned char*) tmp_buf );
	*/
	texture->image.setValue(SbVec2s(_sizeX, _sizeY),1, (const unsigned char*) all_slice_data[0] );
}



libCoin3D::Separator^ libCoin3D::Texture::makeDragerAndTexture(array<array<System::Byte>^> ^data, int axis)
{

	double RES_DEPTH = 1;
	//copy data into local buffer :)
	_all_slice_data1 = new unsigned char*[_sizeZ];
	for (int i=0; i<_sizeZ; i++) {
		_all_slice_data1[i] = new unsigned char[_sizeX*_sizeY];
		for (int j=0; j<_sizeX*_sizeY; j++) {
			unsigned char temp = (unsigned char)data[i][j];
			_all_slice_data1[i][j] = temp;
		}
	}
	
	SoSeparator* separator = new SoSeparator;

	SoScale* myScale = new SoScale();
	SoSeparator* scaleSeparator = new SoSeparator();
	separator->addChild(scaleSeparator);
	scaleSeparator->addChild(myScale);
	myScale->scaleFactor.setValue(6,6,6);
	SoDrawStyle *drawStyle  = new SoDrawStyle;
	drawStyle->style=SoDrawStyle::FILLED;
	scaleSeparator->addChild(drawStyle);


	SoTranslate1Dragger *myDragger = new SoTranslate1Dragger;
	//draggers[axis]=myDragger;	//save a reference to the dragger
	myDragger->translation.setValue(0,0,0);

	SoTransform *myTransform = new SoTransform;
	separator->addChild(myTransform);
	separator->addChild(scaleSeparator);
	scaleSeparator->addChild(myDragger);
   
  

	SoCalculator *myCalc = new SoCalculator;
	myCalc->ref();
	myCalc->A.connectFrom(&myDragger->translation);

	SoCalculator *myCalcTexture = new SoCalculator;
	myCalcTexture->ref();
	myCalcTexture->A.connectFrom(&myDragger -> translation);
	myCalc -> b.setValue( (float)_voxelZ );
	//myCalc -> c.setValue( ACCESS_INDEX_SIGN_I );


	switch ( axis ) 
	{
	case 2:
		myCalc -> a.setValue( (float)_sizeZ ); 
		myCalcTexture -> a.setValue( (float)_sizeZ );
		myCalc->b.setValue( (float)_voxelZ );
		myCalc -> expression = "oA = vec3f(0,0,(6*fabs(A[0])*b) % a)";
		myCalcTexture->expression = "oa = fabs(6*A[0] % a)";
		break;
	default:
	   throw gcnew System::ArgumentException("wrong value for axis in makeDraggerAndTexture()");
	}

	//Make a translation on z;
	SoTranslation *myTranslation = new SoTranslation;

	myTranslation -> translation.connectFrom( &myCalc -> oA);

	// Make  the  MRI group: drawing style + texture + rectangle 
	SoDrawStyle *drawStyleMRI  = new SoDrawStyle;
	drawStyleMRI -> style=SoDrawStyle::FILLED;

	SoTexture2 *texture = new SoTexture2;

	SoSeparator* separatorCT = new SoSeparator;
	separator -> addChild( separatorCT );
	separatorCT -> addChild( drawStyleMRI );
	separatorCT -> addChild( texture );

	char* field_name = "test1";
	SoDB::createGlobalField( field_name, SoSFInt32::getClassTypeId() ); 

	// hook this field to the counter.
	( SoDB::getGlobalField(field_name) )->connectFrom( & myCalcTexture->oa );

	TextureCBData * textureCBdata = new TextureCBData;
	textureCBdata -> axis = axis;
	textureCBdata -> texture = texture;
	textureCBdata -> buffer = _all_slice_data1;
	textureCBdata->sizeX = _sizeX;
	textureCBdata->sizeY = _sizeY;
	textureCBdata->sizeZ = _sizeZ;

	SoFieldSensor* dragger_sensor = new SoFieldSensor( updateTextureCB, textureCBdata );

	dragger_sensor -> attach( SoDB::getGlobalField( field_name ) ); 


	separatorCT -> addChild( myTranslation );

	// Make a  rectangle
	separatorCT -> addChild( makeRectangle( axis ));

   switch( axis ){
   case 2: 
	   //setup the first frame
	   texture->image.setValue(SbVec2s(_sizeX, _sizeY),1, (const unsigned char*) _all_slice_data1[0]);
	   break;
   case 0:
	   //setTextureXplane( texture, _all_slice_data1);
	   break;
   }
	return gcnew Separator(separator);
}

float** libCoin3D::Texture::makeRectangleVertices()
{
	//allocate and set to 0
	_verticesRectangle1 = new float*[4];
	for (int i=0; i<4; i++) {
		_verticesRectangle1[i] = new float[3];
		for (int j=0; j<3; j++)
			_verticesRectangle1[i][j] = 0.0;
	}

	_verticesRectangle1[2][1] = (float)(_sizeY * _voxelY);
	_verticesRectangle1[1][0] = (float)(_sizeX * _voxelX);
	_verticesRectangle1[3][1] = (float)(_sizeY * _voxelY);
	_verticesRectangle1[2][0] = (float)(_sizeX * _voxelX);
	return _verticesRectangle1;
}

SoSeparator* libCoin3D::Texture::makeRectangle(int axis)
{
	SoSeparator *rectangle = new SoSeparator();
	rectangle->ref();


	// Using the new SoVertexProperty node is more efficient
	SoVertexProperty *myVertexProperty = new SoVertexProperty;

	makeRectangleVertices();

	// Define coordinates for vertices
	// vertices is an array of pts of length 4 (each pt has 3 floats; total 12 pts)
	// these vertices define the corner of the texture plane that the CT slices are shown on
	switch( axis ) 
	{
	case 0:  //X
		//myVertexProperty->vertex.setValues(0, 4, vertices_x);
		break;
	case 2:  //Z
		myVertexProperty->vertex.set1Value(0, _verticesRectangle1[0]);
		myVertexProperty->vertex.set1Value(1, _verticesRectangle1[1]);
		myVertexProperty->vertex.set1Value(2, _verticesRectangle1[2]);
		myVertexProperty->vertex.set1Value(3, _verticesRectangle1[3]);
		break;
	}

	// Define the FaceSet
	SoFaceSet *myFaceSet = new SoFaceSet;
	myFaceSet->numVertices.setValue(4);
	
	myFaceSet->vertexProperty.setValue(myVertexProperty);
	rectangle->addChild(myFaceSet);
	
	rectangle->unrefNoDelete();
	return rectangle;
}
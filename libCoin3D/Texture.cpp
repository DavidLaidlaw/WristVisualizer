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
#include <Inventor/events/SoKeyboardEvent.h>
#include <Inventor/events/SoEvent.h>
#include <Inventor/nodes/SoEventCallback.h>

libCoin3D::Texture::Texture(Sides side, int sizeX, int sizeY, int sizeZ, double voxelX, double voxelY, double voxelZ)
{
	_side = side;
	_sizeX = sizeX;
	_sizeY = sizeY;
	_sizeZ = sizeZ;
	_voxelX = voxelX;
	_voxelY = voxelY;
	_voxelZ = voxelZ;
	_draggerXY = NULL;
	_draggerYZ = NULL;
}

libCoin3D::Texture::!Texture()
{
	if (_all_slice_dataXY != NULL) {
		for (int i=0; i<_sizeZ; i++)
			delete _all_slice_dataXY[i];
		delete _all_slice_dataXY;
	}

	if (_all_slice_dataYZ != NULL) {
		for (int i=0; i<_sizeX; i++)
			delete _all_slice_dataYZ[i];
		delete _all_slice_dataYZ;
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

libCoin3D::Separator^ libCoin3D::Texture::createPointsFileObject(array<array<double>^>^ points, array<float>^ color)
{
	return createPointsFileObject(points, color[0], color[1], color[2]);
}
libCoin3D::Separator^ libCoin3D::Texture::createPointsFileObject(cli::array<cli::array<double >^,1> ^points)
{
	return createPointsFileObject(points, 1.0f, 1.0f, 1.0f);
}

libCoin3D::Separator^ libCoin3D::Texture::createPointsFileObject(cli::array<cli::array<double >^,1> ^points, float colorR, float colorG, float colorB)
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

	clr->rgb.setValue(SbColor(colorR, colorG, colorB));

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
  libCoin3D::Texture::Planes plane;
  SoTexture2 * texture; 
  unsigned char** buffer;  
  int sizeX;
  int sizeY;
  int sizeZ;
  double sliceThickness;
  int planeHeight;
  int planeWidth;
  int numSlices;
  SoTranslate1Dragger* dragger;
};	

void updateTextureCB( void * data, SoSensor * )
{
	int xf;
	TextureCBData * textureCBdata  = (TextureCBData *) data;  

	SoTexture2  * texture = textureCBdata->texture;
	unsigned char** buffer = textureCBdata->buffer;

	if ( texture == NULL )
		return;

	libCoin3D::Texture::Planes plane = textureCBdata -> plane;
	SoTranslate1Dragger* dragger = textureCBdata->dragger;
	float dragPos = dragger->translation.getValue()[0];
	float sliceThickness = textureCBdata->sliceThickness;
	float numSlices = (float)textureCBdata->numSlices;

	//determine the index of the image data that we need (in the full buffer)
	xf = (int)fabs(fmod(floor((6*dragPos/sliceThickness)+0.5f),numSlices));
	System::Console::WriteLine("Updating plane to slice: {0} for dragger position {1} (voxels)",xf,(6*dragPos/sliceThickness));
	//set the image to the texture
	texture -> image.setValue(SbVec2s(textureCBdata->planeHeight, textureCBdata->planeWidth),1, (const unsigned char*) buffer[xf] );
}

unsigned char** libCoin3D::Texture::setupLocalBuffer(array<array<System::Byte>^>^ data, Planes plane)
{
	unsigned char** buffer;
	switch (plane) 
	{
	case Planes::XY_PLANE:
		buffer = new unsigned char*[_sizeZ];
		for (int i=0; i<_sizeZ; i++) {
			buffer[i] = new unsigned char[_sizeX*_sizeY];
			if (_sizeY >= _sizeX) {
				/* Need to check the dimensions. For some reason, the SoTexture2 object maps 
				 * itself differently onto the plane depending upon its aspect ratio. It seems
				 * to try and always be oriented in a portrait mode (or square). So we check 
				 * for that case here.
				 */
				System::Runtime::InteropServices::Marshal::Copy((array<unsigned char>^)data[i],0,(System::IntPtr)buffer[i],_sizeX*_sizeY);
			}
			else {
				/* This is the case for landscape, so when creating the buffer, we need to go 
				 * and flip the X & Y coordinates of the image, so it displays correctly
				 */
				for (int j=0; j<_sizeY; j++) {
					for (int k=0; k<_sizeX; k++)
						buffer[i][j*_sizeX + k] = (unsigned char)data[i][k*_sizeY + j];
				}
			}
		}
		break;
	case Planes::YZ_PLANE:
		buffer = new unsigned char*[_sizeX];
		for (int i=0; i<_sizeX; i++) {    //loop through X
			buffer[i] = new unsigned char[_sizeY*_sizeZ];
			if (_sizeZ*_voxelZ >= _sizeY*_voxelY) {
				for (int j=0; j<_sizeZ; j++) {  //loop through Z
					for (int k=0; k<_sizeY; k++) {  //loop through Y
						buffer[i][k*_sizeZ + j] = (unsigned char)data[j][i*_sizeY + k];
					}
				}				
			}
			else {
				for (int j=0; j<_sizeZ; j++) {  //loop through Z
					for (int k=0; k<_sizeY; k++) {  //loop through Y
						buffer[i][j*_sizeY + k] = (unsigned char)data[j][i*_sizeY + k];
					}
				}
			}
		}
		break;
	default:
		throw gcnew System::ArgumentException("Invalid plane", "plane");
	}
	return buffer;
}


libCoin3D::Separator^ libCoin3D::Texture::makeDragerAndTexture(array<array<System::Byte>^> ^data, Planes plane)
{
	//copy data into local buffer :)
	unsigned char** buffer = setupLocalBuffer(data, plane);
	
	SoSeparator* separator = new SoSeparator;

	SoScale* myScale = new SoScale();
	SoSeparator* scaleSeparator = new SoSeparator();
	scaleSeparator->ref();
	separator->addChild(scaleSeparator); //hu
	scaleSeparator->addChild(myScale);
	myScale->scaleFactor.setValue(6,6,6);
	SoDrawStyle *drawStyle  = new SoDrawStyle;
	drawStyle->style=SoDrawStyle::FILLED;
	scaleSeparator->addChild(drawStyle);


	SoTranslate1Dragger *myDragger = new SoTranslate1Dragger;
	myDragger->translation.setValue(0,0,0);

	SoTransform *myTransform = new SoTransform;
	separator->addChild(myTransform);
	//separator->addChild(scaleSeparator);
	scaleSeparator->addChild(myDragger);
   
	SoTexture2 *texture = new SoTexture2;

	TextureCBData * textureCBdata = new TextureCBData;
	textureCBdata->plane = plane;
	textureCBdata->texture = texture;
	textureCBdata->buffer = buffer;
	textureCBdata->sizeX = _sizeX;
	textureCBdata->sizeY = _sizeY;
	textureCBdata->sizeZ = _sizeZ;
	
	textureCBdata->dragger = myDragger;

	SoCalculator *myCalc = new SoCalculator;
	myCalc->ref();
	myCalc->A.connectFrom(&myDragger->translation);
	//myCalc -> b.setValue( (float)_voxelZ );
	//myCalc -> c.setValue( ACCESS_INDEX_SIGN_I );

	switch ( plane ) 
	{
	case Planes::XY_PLANE:
		_all_slice_dataXY = buffer;
		myCalc -> a.setValue( (float)_sizeZ ); 
		//myCalc -> expression = "oA = vec3f(0,0,(floor(6*fabs(A[0]))) % a)";
		myCalc -> c.setValue( _voxelZ );
		myCalc -> expression = "oA = vec3f(0,0,(floor((6*fabs(A[0])/c)+0.5) % a) * c + c/2)";
		textureCBdata->sliceThickness = _voxelZ;
		textureCBdata->numSlices = _sizeZ;
		textureCBdata->planeHeight = _sizeY;
		textureCBdata->planeWidth = _sizeX;
		if (_sizeY < _sizeX) {
			/* Need to check for cases where we are wider then taller
			 * Due to a problem with SoTexture2, images are always display in
			 * portrain mode. For more info, see comments in function setupLocalBuffer()
			 */
			textureCBdata->planeHeight = _sizeX;
			textureCBdata->planeWidth = _sizeY;
		}
		_draggerXY = myDragger; //save reference to this
		break;
	case Planes::YZ_PLANE:
		_all_slice_dataYZ = buffer;
		myCalc -> a.setValue( (float)_sizeX );
		myCalc -> c.setValue( _voxelX ); //TODO: Fix?
		myCalc -> expression = "oA = vec3f((floor((6*fabs(A[0])/c)+0.5) % a)*c +c/2, 0,0)";
		textureCBdata->sliceThickness = _voxelX;
		textureCBdata->numSlices = _sizeX;
		textureCBdata->planeHeight = _sizeZ;
		textureCBdata->planeWidth = _sizeY;
		if (_sizeZ*_voxelZ < _sizeY*_voxelY) {
			textureCBdata->planeHeight = _sizeY;
			textureCBdata->planeWidth = _sizeZ;
		}
		_draggerYZ = myDragger;
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

	SoSeparator* separatorCT = new SoSeparator;
	separator -> addChild( separatorCT );
	separatorCT -> addChild( drawStyleMRI );
	separatorCT -> addChild( texture );


	//create a sensor, and attach it to the output from our translation,
	//so that we know when our dragger is moved
	SoFieldSensor* dragger_sensor = new SoFieldSensor( updateTextureCB, textureCBdata );
	dragger_sensor -> attach(&myTranslation->translation);

	separatorCT -> addChild( myTranslation );

	// Make a  rectangle
	separatorCT -> addChild( makeRectangle( plane ));

	//show the image for the first slice
	updateTextureCB((void*)textureCBdata,NULL);

	return gcnew Separator(separator);
}

SoSeparator* libCoin3D::Texture::makeRectangle(Planes plane)
{
	SoSeparator *rectangle = new SoSeparator();
	rectangle->ref();


	// Using the new SoVertexProperty node is more efficient
	SoVertexProperty *myVertexProperty = new SoVertexProperty;

	// Define coordinates for vertices
	// vertices is an array of pts of length 4 (each pt has 3 floats; total 12 pts)
	// these vertices define the corner of the texture plane that the CT slices are shown on
	switch( plane ) 
	{
	case Planes::XY_PLANE:  //Z
		myVertexProperty->vertex.set1Value(0, 0.0f, 0.0f, 0.0f);
		myVertexProperty->vertex.set1Value(1, (float)(_sizeX*_voxelX),0.0f,0.0f);
		myVertexProperty->vertex.set1Value(2, (float)(_sizeX*_voxelX), (float)(_sizeY*_voxelY), 0.0f);
		myVertexProperty->vertex.set1Value(3, 0.0f, (float)(_sizeY*_voxelY), 0.0f);
		break;
	case Planes::YZ_PLANE:  //X
		myVertexProperty->vertex.set1Value(0, 0.0f, 0.0f, 0.0f);
		myVertexProperty->vertex.set1Value(1, 0.0f, (float)(_sizeY*_voxelY), 0.0f);
		myVertexProperty->vertex.set1Value(2, 0.0f, (float)(_sizeY*_voxelY), (float)(_sizeZ*_voxelZ));
		myVertexProperty->vertex.set1Value(3, 0.0f, 0.0f, (float)(_sizeZ*_voxelZ));
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

void myKeyPressCB( void * userData, SoEventCallback * eventCB )
{
	int HWND = (int)userData;
	libCoin3D::Texture^ texture = libCoin3D::Texture::getTextureByParentWidget(HWND);
	if (texture==nullptr) //help
		return; 
	const SoEvent * event = eventCB->getEvent();
	

	if (SO_KEY_PRESS_EVENT(event, UP_ARROW)) { //Move through the slices in the Z coord
		texture->moveDragger(libCoin3D::Texture::Planes::XY_PLANE,1);
	}
	if (SO_KEY_PRESS_EVENT(event, DOWN_ARROW)) { //Move through the slices in the Z coord
		texture->moveDragger(libCoin3D::Texture::Planes::XY_PLANE,-1);
	}
    if (SO_KEY_PRESS_EVENT(event, LEFT_ARROW)) { //Move through the slices in the Y coord
		texture->moveDragger(libCoin3D::Texture::Planes::YZ_PLANE,-1);
		eventCB->setHandled();
	}
	if (SO_KEY_PRESS_EVENT(event, RIGHT_ARROW)) { //Move through the slices in the Y coord
		texture->moveDragger(libCoin3D::Texture::Planes::YZ_PLANE,1);
		//System::Console::WriteLine("RIGHT");
		eventCB->setHandled();
	}
}


libCoin3D::Separator^ libCoin3D::Texture::createKeyboardCallbackObject(int viewerParrentHWND)
{
	TexturesHashtable[viewerParrentHWND] = this;

	SoEventCallback * myEventCB = new SoEventCallback;
	myEventCB->addEventCallback( SoKeyboardEvent::getClassTypeId(),
		myKeyPressCB, (void*)viewerParrentHWND);
	SoSeparator* sep = new SoSeparator();
	Separator^ wrapper = gcnew Separator(sep);
	sep->addChild(myEventCB);
	return wrapper;
}

void libCoin3D::Texture::moveDragger(Planes plane,int howFar) {
	SoTranslate1Dragger* dragger;
	switch (plane) 
	{
	case Planes::XY_PLANE:
		dragger = _draggerXY;
		break;
	case Planes::YZ_PLANE:
		dragger = _draggerYZ;
		break;
	default:
		throw gcnew System::ArgumentException("What the fuck");
	}
	
	if (!dragger) return;  //ignore if we are not set yet
    
	float x,y,z;
	dragger->translation.getValue().getValue(x,y,z);
	dragger->translation.setValue(x+(howFar/6.),0,0);
}

libCoin3D::Texture^ libCoin3D::Texture::getTextureByParentWidget(int HWND)
{
	if (TexturesHashtable == nullptr)
		return nullptr; //this should never happen, but just in case
	return (Texture^)TexturesHashtable[HWND]; //return it
}
#include "StdAfx.h"
#include "TranslateDragger.h"
#include "TextureCBData.h"
#include <Inventor\nodes\SoCube.h>
#include <Inventor/nodes/SoTexture2.h>
#include <Inventor/engines/SoCalculator.h>
#include <math.h>



libCoin3D::TranslateDragger::TranslateDragger():
  _draggerXY(nullptr),
  _draggerYZ(nullptr)
{
	_translateDragger = new SoTranslate1Dragger();
	_translateDragger->ref();



}


libCoin3D::TranslateDragger::~TranslateDragger()
{
	_translateDragger->unref();
	
}

libCoin3D::TranslateDragger::TranslateDragger(Texture^ texture){
    setDraggerXY(texture->get_draggerXY());
    setDraggerYZ(texture->get_draggerYZ());
    _texture=texture;	    
}

libCoin3D::TranslateDragger::TranslateDragger(SoTranslate1Dragger* translateDragger)
{
	_translateDragger = translateDragger;
	if (_translateDragger==NULL)
		_translateDragger = new SoTranslate1Dragger();
}
void
libCoin3D::TranslateDragger::updateTrackbar(Texture^ texture,int value, libCoin3D::Texture::Planes plane){
    
    
    int xf; 
    switch(plane){
     
	case libCoin3D::Texture::Planes::YZ_PLANE:
	texture->_draggerXY->translation.setValue(value,0,0);
	updateTextureCB(texture->textureCBdataXY, value, NULL);
	
	  break;
       case libCoin3D::Texture::Planes::XY_PLANE:
	
	texture->_draggerYZ->translation.setValue(value,0,0);
	updateTextureCB(texture->textureCBdataYZ, value, NULL);

	break;
      default:
	throw gcnew System::ArgumentException("Unknown value for plane in TranslateDragger::updateTrackbar");
    }
   
}
void 
libCoin3D::TranslateDragger::updateTextureCB( void * data,int dragPosition1,  SoSensor * )
{
	int xf;
	TextureCBData * textureCBdata  = (TextureCBData *) data;  
	int dragPosition = dragPosition1*textureCBdata->sliceThickness;
	SoTexture2  * texture = textureCBdata->texture;
	unsigned char** buffer = textureCBdata->buffer;

	if ( texture == NULL )
		return;

	libCoin3D::Texture::Planes plane = textureCBdata -> plane;
	SoTranslate1Dragger* dragger = textureCBdata->dragger;
	float dragPos = (float)dragPosition;
	float sliceThickness = textureCBdata->sliceThickness;
	float numSlices = (float)textureCBdata->numSlices;

	//determine the index of the image data that we need (in the full buffer)
	xf = (int)fabs(fmod(floor((6*dragPos/sliceThickness)+0.5f),numSlices));
	//System::Console::WriteLine("Updating plane to slice: {0}=value  {1} for dragger position {2} (voxels)",dragPos,xf,(6*dragPos/sliceThickness));
	//set the image to the texture
	texture -> image.setValue(SbVec2s(textureCBdata->planeHeight, textureCBdata->planeWidth),1, (const unsigned char*) buffer[xf] );
}




//delegate void tempCB( void * userData, SoDragger *dragger);




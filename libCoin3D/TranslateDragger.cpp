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
	// trackbar1 left-right
	case libCoin3D::Texture::Planes::YZ_PLANE:
	  //texture->_draggerYZ->translation.setValue(value,0,0);
	updateTextureCB(texture->textureCBdataYZ, value, NULL);
	
	  break;
	  // trackbar2 up-down
       case libCoin3D::Texture::Planes::XY_PLANE:
	
	 //texture->_draggerXY->translation.setValue(value,0,0);
	updateTextureCB(texture->textureCBdataXY, value, NULL);

	break;
      default:
	throw gcnew System::ArgumentException("Unknown value for plane in TranslateDragger::updateTrackbar");
    }
   
}


void 
libCoin3D::TranslateDragger::updateTextureCB( void * data,int sliceIndex,  SoSensor * )
{
	int xf;
	TextureCBData * textureCBdata  = (TextureCBData *) data;  
	SoTexture2  * texture = textureCBdata->texture;
	unsigned char** buffer = textureCBdata->buffer;

	if ( texture == NULL )
		return;

	libCoin3D::Texture::Planes plane = textureCBdata -> plane;
	SoTranslate1Dragger* dragger = textureCBdata->dragger;
	
	float sliceThickness = textureCBdata->sliceThickness;
	float numSlices = (float)textureCBdata->numSlices;

	// set the translation of the dragger and the callback to
	// texture will take care of setting the slice This means
	// mapping slice index back into voxel-based coords with which
	// dragger position is represented
	float dragPos = ((float)sliceIndex-0.5f)*(sliceThickness/6.0);
	dragger->translation.setValue(dragPos,0,0);
	

}


//delegate void tempCB( void * userData, SoDragger *dragger);




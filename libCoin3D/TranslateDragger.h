#pragma once

#include "Node.h"
#include <Inventor/nodes/SoNode.h>
#include <Inventor/draggers/SoTranslate1Dragger.h>
#include "Texture.h"
class TextureCBData;

namespace libCoin3D {
public ref class TranslateDragger : Node
{
 public:
    TranslateDragger();
    TranslateDragger(Texture^ texture);
    ~TranslateDragger();
    TranslateDragger(SoTranslate1Dragger* translateDragger);
  
    delegate void delFunc(int);
   
    void updateTrackbar(Texture^ texture, int value, libCoin3D::Texture::Planes plane);
	
    void updateTextureCB( void * data, int dragPosition, SoSensor * );
    
    SoNode* getNode() override
    {
	return nullptr;
    }
 public:
    void setDraggerXY(SoTranslate1Dragger* dragger)
    {
	_draggerXY= dragger;
    }
    void setDraggerYZ(SoTranslate1Dragger* dragger)
    {
	_draggerYZ= dragger;
    }
    Texture^ getTexture(){return _texture;}
 private: 
    SoTranslate1Dragger* _draggerXY;
    SoTranslate1Dragger* _draggerYZ;
   
    Texture^ _texture;
    // unused for now 
    SoTranslate1Dragger* _translateDragger;
    static delFunc^ _CSharpCallBack;
 
};
}


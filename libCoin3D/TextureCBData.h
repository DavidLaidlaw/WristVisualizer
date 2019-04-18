#pragma once

#include "Node.h"
#include <Inventor/nodes/SoNode.h>
#include <Inventor/draggers/SoTranslate1Dragger.h>
#include "Texture.h"

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

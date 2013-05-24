#include "StdAfx.h"
#include "TextureHandler.h"

#include <stdio.h>

TextureHandler::TextureHandler(void)
{
}


TextureHandler::~TextureHandler(void)
{
}

GLuint* TextureHandler::create3DTextureBonePreview(){
	GLuint* bindLocation;
	glGenTextures(6, bindLocation);

	int w,h,l;
	w=0;
	h=0;
	l=0;
	//--------------------------------------------------------------
	//REGULAR DATA
	//--------------------------------------------------------------
	glBindTexture(GL_TEXTURE_3D, bindLocation[0]);//-create or use a named texture
	glPixelStorei(GL_UNPACK_ALIGNMENT, 1);//-byte alignment
	glTexParameteri(GL_TEXTURE_3D, GL_TEXTURE_WRAP_S, GL_CLAMP);
	glTexParameteri(GL_TEXTURE_3D, GL_TEXTURE_WRAP_T, GL_CLAMP);
	glTexParameteri(GL_TEXTURE_3D, GL_TEXTURE_WRAP_R, GL_CLAMP);
	glTexParameteri(GL_TEXTURE_3D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_3D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	glTexImage3D( GL_TEXTURE_3D, 0,GL_RGBA,//GL_RGBA8_SNORM
		w, h, l, 0,
		GL_RGBA, GL_UNSIGNED_BYTE, 0 );//GL_UNSIGNED_BYTE--THE TEXTURE BYTES NEED TO BE IN THIS LAST PLACE
	printf("Done creating texture\n");

	return bindLocation;
}

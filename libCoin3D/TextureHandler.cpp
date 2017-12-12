#include "StdAfx.h"
#include "TextureHandler.h"

#include <stdio.h>

TextureHandler::TextureHandler(void)
{
}


TextureHandler::~TextureHandler(void)
{
}

GLuint TextureHandler::create3DTextureBonePreview(int* d, Vector3 length){
	//COIN_GLERROR_DEBUGGING=1;

	int w=length.x;
	int h=length.y;
	int l=length.z;

	GLuint bindLocation;
	glGenTextures(1, &bindLocation);

	//		for(int i=0;i<w*h*l*4;i++){
	//	if(d[i]>100){printf("some data msc %i \n",d[i]);fflush(stdout);}
	//}


	char *ptr=static_cast<char*>(static_cast<void*>(d));

	printf("data size from c++%i \n",(w*h*l*4));fflush(stdout);
	glBindTexture(GL_TEXTURE_3D, bindLocation);//-create or use a named texture
	glPixelStorei(GL_UNPACK_ALIGNMENT, 1);//-byte alignment
	glTexParameteri(GL_TEXTURE_3D, GL_TEXTURE_WRAP_S, GL_CLAMP);
	glTexParameteri(GL_TEXTURE_3D, GL_TEXTURE_WRAP_T, GL_CLAMP);
	glTexParameteri(GL_TEXTURE_3D, GL_TEXTURE_WRAP_R, GL_CLAMP);
	glTexParameteri(GL_TEXTURE_3D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_3D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	//printf("some data %i %i %i %i\n",d[0],d[1],d[2],d[3]);fflush(stdout);

	//next try printing withut casting


	glTexImage3D( GL_TEXTURE_3D, 0,GL_RGBA8, w, h, l, 0,GL_RGBA, GL_UNSIGNED_BYTE,ptr );

	printf("Done creating texture for bind loc %i\n",bindLocation);fflush(stdout);

	return bindLocation;
}

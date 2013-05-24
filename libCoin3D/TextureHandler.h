#pragma once

#include "glew\glew-1.9.0\include\GL\glew.h"
#include <gl\GL.h>

class TextureHandler
{
public:
	TextureHandler(void);
	~TextureHandler(void);

	/*
	 *  the binding location of the texture, and dimensions(width,height,length))
	 */
	GLuint* create3DTextureBonePreview();

};


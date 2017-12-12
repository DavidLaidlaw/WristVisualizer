#pragma once
#include <Windows.h>
#include "x64\glew-1.9.0\include\GL\glew.h"
#include <gl\GL.h>
#include "Vector.h"

class TextureHandler
{
public:
	TextureHandler(void);
	~TextureHandler(void);

	/*
	 *  the binding location of the texture, and dimensions(width,height,length))
	 */
	GLuint create3DTextureBonePreview(int* d, Vector3 length);

};


#pragma once
//#include <iostream>     // std::cout, std::ios
//#include <sstream>      // std::stringstream
//#include <istream>
#include <fstream>

#include <Windows.h>
#include "x64\glew-1.9.0\include\GL\glew.h"
#include <gl\GL.h>
using namespace std;

class ShaderHandler
{
public:
	ShaderHandler(void);
	virtual ~ShaderHandler(void);
	GLuint load(std::string path, GLenum shaderType);
	void printShaderLog(GLuint shader);
	void printProgramLog( GLuint program );

	bool loadTheShaders();

	int useShader();
	void dontUseShader();
private:
	GLuint mProgramID;

};


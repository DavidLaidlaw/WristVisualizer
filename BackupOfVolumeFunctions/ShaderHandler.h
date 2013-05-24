#pragma once
//#include <iostream>     // std::cout, std::ios
//#include <sstream>      // std::stringstream
//#include <istream>
#include <fstream>

#include <Windows.h>
#include <GL/glew.h>


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

	int useShader()
	{
		glUseProgram(mProgramID);
		return mProgramID;
	}

	void dontUseShader()
	{
		glUseProgram(0);
	}

private:
	GLuint mProgramID;

};


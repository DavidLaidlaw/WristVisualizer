#include "StdAfx.h"
#include "ShaderHandler.h"
#include <string>
#include <ios>

ShaderHandler::ShaderHandler(void)
{
	//as soon as the shader handler is created, 
	//load the shaders I need to use--done in master cube constructor

}


ShaderHandler::~ShaderHandler(void)
{
}

bool ShaderHandler::loadTheShaders(){
	//Generate program
	mProgramID = glCreateProgram();

	//Load vertex shader
	GLuint vertexShader = load( "v.vsh", GL_VERTEX_SHADER );

	//Check for errors
	if( vertexShader == 0 )
	{
		glDeleteProgram( mProgramID );
		mProgramID = 0;
		return false;
	}

	//Attach vertex shader to program
	glAttachShader( mProgramID, vertexShader );


	//Create fragment shader
	GLuint fragmentShader = load( "f.fsh", GL_FRAGMENT_SHADER );

	//Check for errors
	if( fragmentShader == 0 )
	{
		glDeleteProgram( mProgramID );
		mProgramID = 0;
		return false;
	}

	//Attach fragment shader to program
	glAttachShader( mProgramID, fragmentShader );

	//Link program
	glLinkProgram( mProgramID );

	//Check for errors
	GLint programSuccess = GL_TRUE;
	glGetProgramiv( mProgramID, GL_LINK_STATUS, &programSuccess );
	if( programSuccess != GL_TRUE )
	{
		printf( "Error linking program %d!\n", mProgramID );
		printProgramLog( mProgramID );
		glDeleteProgram( mProgramID );
		mProgramID = 0;
		return false;
	}

	return true;
}


void ShaderHandler::printProgramLog( GLuint program ){
    //Make sure name is shader
    if( glIsProgram( program ) )
    {
        //Program log length
        int infoLogLength = 0;
        int maxLength = infoLogLength;

        //Get info string length
        glGetProgramiv( program, GL_INFO_LOG_LENGTH, &maxLength );

        //Allocate string
        char* infoLog = new char[ maxLength ];

        //Get info log
        glGetProgramInfoLog( program, maxLength, &infoLogLength, infoLog );
        if( infoLogLength > 0 )
        {
            //Print Log
            printf( "%s\n", infoLog );
        }

        //Deallocate string
        delete[] infoLog;
    }
    else
    {
        printf( "Name %d is not a program\n", program );
    }
}

GLuint ShaderHandler::load(std::string path, GLenum shaderType){


	//Open file
	GLuint shaderID = 0;
	std::string shaderString;
	std::ifstream sourceFile( path.c_str() );

	//Source file loaded
	if( sourceFile )
	{
		//Get shader source
		shaderString.assign( ( std::istreambuf_iterator< char >( sourceFile ) ), std::istreambuf_iterator< char >() );


		//Create shader ID
		shaderID = glCreateShader( shaderType );

		//Set shader source
		const GLchar* shaderSource = shaderString.c_str();
		glShaderSource( shaderID, 1, (const GLchar**)&shaderSource, NULL );

		//Compile shader source
		glCompileShader( shaderID );

		//Check shader for errors
		GLint shaderCompiled = GL_FALSE;
		glGetShaderiv( shaderID, GL_COMPILE_STATUS, &shaderCompiled );
		if( shaderCompiled != GL_TRUE )
		{
			printf( "Unable to compile shader %d!\n\nSource:\n%s\n", shaderID, shaderSource );
			printShaderLog( shaderID );
			glDeleteShader( shaderID );
			shaderID = 0;
		}
	}
	else
	{
		printf( "Unable to open file %s\n", path.c_str() );
	}

	return shaderID;
}



void ShaderHandler::printShaderLog( GLuint shader ){
	//Make sure name is shader
	if( glIsShader( shader ) )
	{
		//Shader log length
		int infoLogLength = 0;
		int maxLength = infoLogLength;

		//Get info string length
		glGetShaderiv( shader, GL_INFO_LOG_LENGTH, &maxLength );

		//Allocate string
		char* infoLog = new char[ maxLength ];

		//Get info log
		glGetShaderInfoLog( shader, maxLength, &infoLogLength, infoLog );
		if( infoLogLength > 0 )
		{
			//Print Log
			printf( "%s\n", infoLog );
		}

		//Deallocate string
		delete[] infoLog;
	}
	else
	{
		printf( "Name %d is not a shader\n", shader );
	}
}
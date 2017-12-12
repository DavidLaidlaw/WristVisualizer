#include "StdAfx.h"
#include "MasterCube.h"
#include <Windows.h>
#include "glew\glew-1.9.0\include\GL\glew.h"
#include <gl\GL.h>
#include <iostream>

MasterCube::MasterCube(int w,int h, int l,int x,int y,int z, int* d, bool IsLeft)
{
	doDrawVolume=false;	
	glewIsInit=false;
	data=new int[x*y*z*4];
	isLeft=IsLeft;

	length=Vector3(w,h,l);
	voxSize=Vector3(x,y,z);

	//it is necessary to actually copy this data because the actual data appears to be released by gc in c#
	for(int i=0;i<voxSize.x*voxSize.y*voxSize.z*4;i++){
		data[i]=d[i];
	}

	//number of planes to use in texture render
	numProxies=50;

	origNumSlices=x;//passed into shader for opacity stuff
	max=0.5;
	curr=0;
	opacityValue=0.5;
	threshValue=0.1;
	startThresh=0.3;
	isOpaque=0;
	calcNormsOnFly=1;
	drawOutlines=true;
	drawTexture=true;
	colorByNormals=0;


	cube=CubeGeometry(Vector3(0,0,0),length);
	//eval;
	vector<ProxyPiece> proxyPieces;

	//then the texture handler will need to handle the textur
	textureSize=length;//just for holding place
}


MasterCube::~MasterCube(void)
{

}


void MasterCube::setDoDrawVolume(bool b){
	doDrawVolume=b;
}

//the following two functions are the same, sadly
void MasterCube::setSliceNum(int num){
		numProxies=num;
}
void MasterCube::setProxNum(int num){
	numProxies=num;
}

void MasterCube::setIsOpaque(bool b){
	if(b) isOpaque=1;
	else isOpaque=0;
}


void MasterCube::setOpacity(float o){
	opacityValue=o;
}


void MasterCube::initializeGlew(){

	shader.loadTheShaders();
	//read in textures
	boneBindingLoc = texHandler.create3DTextureBonePreview(data,voxSize);
}

void MasterCube::drawVolumes(){
	if(doDrawVolume){
		glEnable(GL_DEPTH_TEST);
		glPushMatrix();	

		glPolygonMode( GL_FRONT_AND_BACK, GL_LINE);
		glTexParameteri( GL_TEXTURE_3D, GL_TEXTURE_MIN_FILTER, GL_NEAREST_MIPMAP_NEAREST);
		float xVal=length.x/2;
		if(isLeft){
			xVal=-xVal;
		}
		glTranslatef(xVal,length.y/2,length.z/2);


		//for debugging you can uncomment this to see cube edges
		//if(drawOutlines){
		//	cube.draw();
		//	this->createAndDrawProxyPieces();
		//}

		glColor4f(1,0,0,1);
		glEnable( GL_CULL_FACE);

		glPolygonMode( GL_FRONT_AND_BACK, GL_FILL);
		glTexParameteri( GL_TEXTURE_3D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);		
		glEnable( GL_BLEND);
		glBlendFunc ( GL_SRC_ALPHA,  GL_ONE_MINUS_SRC_ALPHA);

		if(drawTexture){
			this->enableTexturing();
			this->createAndDrawProxyPieces();
			this->disableTexturing();
		}
		glPopMatrix();
	}
}


void MasterCube::enableTexturing(){
	Vector3 lightLoc=this->getViewingAngle();

	int sp=shader.useShader(); //returns shader program number		
	int testValueLoc =  glGetUniformLocation(sp, "testValue");
	glUniform1f(testValueLoc,opacityValue);

	int isOpaqueLoc =  glGetUniformLocation(sp, "isOpaque");
	glUniform1i(isOpaqueLoc,isOpaque);

	int threshValueLoc =  glGetUniformLocation(sp, "threshValue");
	glUniform1f(threshValueLoc,threshValue);

	int startThreshValueLoc =  glGetUniformLocation(sp, "startThresh");
	glUniform1f(startThreshValueLoc,startThresh);

	int drawNormalLoc =  glGetUniformLocation(sp, "viewNormals");
	glUniform1i(drawNormalLoc,colorByNormals);

	int calcNormsOnFlyLoc =  glGetUniformLocation(sp, "calcNormsOnFly");
	glUniform1i(calcNormsOnFlyLoc,calcNormsOnFly);

	int lightPositionLoc =  glGetUniformLocation(sp, "LightPosition");
	glUniform3f(lightPositionLoc,lightLoc.x,lightLoc.y,lightLoc.z);

	int numProxiesLoc =  glGetUniformLocation(sp, "numProxies");
	glUniform1f(numProxiesLoc,(float)numProxies);

	int dimLoc =  glGetUniformLocation(sp, "dim");
	glUniform3f(dimLoc,textureSize.x,textureSize.y,textureSize.z);

	int origNumProxiesLoc =  glGetUniformLocation(sp, "origNumProxies");
	glUniform1f(origNumProxiesLoc,origNumSlices);

	glActiveTexture( GL_TEXTURE1);
	int texLoc3d= glGetUniformLocation(sp, "textureVar3D");
	glUniform1i(texLoc3d,1);
	glBindTexture( GL_TEXTURE_3D, boneBindingLoc);
	glActiveTexture( GL_TEXTURE0);
}


void MasterCube::disableTexturing(){
	shader.dontUseShader();
}


void MasterCube::createAndDrawProxyPieces(){
	Vector3 viewDir=this->getViewingAngle();
	Vector3 startLoc=viewDir;
	Vector3 endLoc=viewDir*cube.getLength();//not actually used//is it even right??
	proxyPieces.clear();
	vector<ProxyPiece> newPieces=eval.createProxyPieces(cube, viewDir,startLoc, endLoc, numProxies);
	proxyPieces=eval.createProxyPieces(cube, viewDir,startLoc, endLoc, numProxies);
	//iterate through num=size, because some proxies might be dropped
	for(int i=0;i<newPieces.size();i++){
		proxyPieces.insert(proxyPieces.begin()+i,newPieces.at(i));
	}

	//for now just draw with polygons, doesn't make a noticeable difference
	//triangle drawing code exists in the Java volume renderer
	for(int i=0;i<proxyPieces.size();i++){
		proxyPieces.at(i).draw(isLeft);
		//}
	}
}

void MasterCube::renderCube(){
	//if glew didn't init then init glew!
	if(!glewIsInit){
		glewExperimental=GL_TRUE;
		GLenum err = glewInit(); 
		if (GLEW_OK != err)
		{
			/* glewInit failed and you probably want to kill yourself */
			fprintf(stderr, "Error1: %s\n", glewGetErrorString(err));
		}
		printf("Error2: %s\n", glewGetErrorString(err));
		fprintf(stdout, "Status: Using GLEW %s\n", glewGetString(GLEW_VERSION));fflush(stdout);
		if(err==GLEW_OK){
			glewIsInit=true;
			//a method to initialize glew
			printf("going to initilize glew\n");fflush(stdout);
			initializeGlew();
		}
	}

	glPushAttrib(GL_CULL_FACE);
	glPushAttrib(GL_LIGHTING);
	glPushAttrib(GL_BLEND);

	glPushMatrix();
	glTranslatef(0.0, 0.0, 0.0);
	glLineWidth(2);
	glDisable(GL_LIGHTING); // so we don’t have to set normals
	glDisable(GL_CULL_FACE);

	glEnable(GL_BLEND);
	glBlendFunc (GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

	//time to draw volumes
	drawVolumes();

	glLineWidth(1);
	glPopMatrix();

	glPopAttrib();//cull face
	glPopAttrib();//lighting
	glPopAttrib();//blend
}


Vector3 MasterCube::getViewingAngle(){
	GLfloat mat[4*4];
	glGetFloatv( GL_MODELVIEW_MATRIX, mat );
	//return Vector3(0,0,-1);
	return Vector3(mat[2],mat[6],mat[10]);//Z-AXIS!!
	/*
	0123
	4567
	8901
	2345*/
}
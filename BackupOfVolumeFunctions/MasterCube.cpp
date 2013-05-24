#include "StdAfx.h"
#include "MasterCube.h"
#include <Windows.h>
#include <GL/glew.h>
#include <iostream>

MasterCube::MasterCube(void)
{
	//initialize glew here
	//will need it for shader loading
	glewInit();


	numProxies=100;
	origNumSlices=21;//will need to take as input
	max=0.5;
	curr=0;
	opacityValue=1.0;
	threshValue=0.1;
	startThresh=0.3;
	isOpaque=1;
	calcNormsOnFly=1;
	drawOutlines=true;
	drawTexture=false;
	colorByNormals=0;


	cube=CubeGeometry(Vector3(0,0,0),2);
	//eval;
	//vector<ProxyPiece> proxyPieces;

	//now time for shader setup

	shader.loadTheShaders();
	//then the texture handler will need to handle the textur
	textureSize=Vector3(1,2,3);//just for holding place
}


MasterCube::~MasterCube(void)
{
}


void MasterCube::drawVolumes(){
	glEnable(GL_DEPTH_TEST);
	glPushMatrix();
	glLoadIdentity();			

	glPolygonMode( GL_FRONT_AND_BACK, GL_LINE);
	glTexParameteri( GL_TEXTURE_3D, GL_TEXTURE_MIN_FILTER, GL_NEAREST_MIPMAP_NEAREST);
	/*glTranslatef(0.0f,0.0f,-5.0f);
	glRotated(rotations.y, 1.0, 0.0, 0.0);
	glRotated(rotations.x, 0.0, 1.0, 0.0);
	*/
	//don't yet know where to place it
	if(drawOutlines){
		cube.draw();
		this->createAndDrawProxyPieces();
	}

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
	//vector<ProxyPiece> newPieces=eval.createProxyPieces(cube, viewDir,startLoc, endLoc, numProxies);
	proxyPieces=eval.createProxyPieces(cube, viewDir,startLoc, endLoc, numProxies);
	//iterate through num=size, because some proxies might be dropped
	//for(int i=0;i<newPieces.size();i++){
	//	proxyPieces.insert(proxyPieces.begin+i,newPieces.at(i));
	//}
	////and always draw
	/*if(drawMethod==DrawMethod.triangles){
	glBegin(GL2.GL_TRIANGLES);
	for(int i=0;i<proxyPieces.size();i++){
	proxyPieces.at(i).draw();
	}
	glEnd();
	}*/
	//else if(drawMethod==DrawMethod.polygons){
	for(int i=0;i<proxyPieces.size();i++){
		proxyPieces.at(i).draw();
	}
	//}
}

void MasterCube::renderCube(){

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

	//GLfloat matrix[16]; 
	//glGetFloatv (GL_MODELVIEW_MATRIX, matrix);
	//printf("%f %f %f %f\n",matrix[0],matrix[1],matrix[2],matrix[3]);
	//printf("%f %f %f %f\n",matrix[4],matrix[5],matrix[6],matrix[5]);
	//printf("%f %f %f %f\n",matrix[8],matrix[9],matrix[10],matrix[11]);
	//printf("%f %f %f %f\n",matrix[12],matrix[13],matrix[14],matrix[15]);

	glBegin(GL_QUADS);
	glColor4f(0, 0.7, 0,0.9);glVertex3f(150,0,0);
	glColor4f(0.7,0, 0,0.9);glVertex3f(150,150,120);
	glColor4f(0.7, 0.7,0,0.9);glVertex3f(0,150,120);
	glColor4f(0, 0.7, 0.7,0.9);glVertex3f(0,0,0);
	glEnd();
	glColor4f(1,1,1,1);

	glLineWidth(1);
	glPopMatrix();

	glPopAttrib();//cull face
	glPopAttrib();//lighting
	glPopAttrib();//blend
}


Vector3 MasterCube::getViewingAngle(){
	return Vector3(0,0,-1);
}
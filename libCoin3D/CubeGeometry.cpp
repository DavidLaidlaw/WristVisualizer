#include "StdAfx.h"
#include "CubeGeometry.h"
#include <Windows.h>
#include "glew\glew-1.9.0\include\GL\glew.h"
#include <gl\GL.h>

CubeGeometry::CubeGeometry(Vector3 c, Vector3 l)
{
	center=c;
	length=l;//size of cube to be drawn
	halfLength=length/2;
	setupCornersAndSegments();
}


//don't use this default constructor, its gross and a 2x2x2 cube is unlikely to happen
CubeGeometry::CubeGeometry()
{
	center=Vector3(0,0,0);
	length=Vector3(2,2,2);//size of cube to be drawn
	halfLength=length/2;
	setupCornersAndSegments();
}

CubeGeometry::~CubeGeometry(void)
{
}


//store values for corners and edge segments of the cube, for doing math on it later
void CubeGeometry::setupCornersAndSegments(){
	Vector3 hL=halfLength;
	Vector3 c=center;

	//a margin of 3 seems to work well in cutting off edge noise
	int margin=3;

	//establish the corners of the volume's containing cube, cuts of the margin areas
	corners[0]=Vector3(c.x-hL.x+margin,c.y-hL.y+margin,c.z+hL.z-margin);
	corners[1]=Vector3(c.x-hL.x+margin,c.y+hL.y-margin,c.z+hL.z-margin);
	corners[2]=Vector3(c.x+hL.x-margin,c.y+hL.y-margin,c.z+hL.z-margin);
	corners[3]=Vector3(c.x+hL.x-margin,c.y-hL.y+margin,c.z+hL.z-margin);

	corners[4]=Vector3(c.x-hL.x+margin,c.y-hL.y+margin,c.z-hL.z+margin);
	corners[5]=Vector3(c.x-hL.x+margin,c.y+hL.y-margin,c.z-hL.z+margin);
	corners[6]=Vector3(c.x+hL.x-margin,c.y+hL.y-margin,c.z-hL.z+margin);
	corners[7]=Vector3(c.x+hL.x-margin,c.y-hL.y+margin,c.z-hL.z+margin);


	segments[0]=Segment(corners[0],corners[1]);
	segments[1]=Segment(corners[0],corners[3]);
	segments[2]=Segment(corners[0],corners[4]);
	segments[3]=Segment(corners[4],corners[7]);
	segments[4]=Segment(corners[4],corners[5]);
	segments[5]=Segment(corners[5],corners[1]);
	segments[6]=Segment(corners[5],corners[6]);
	segments[7]=Segment(corners[6],corners[7]);
	segments[8]=Segment(corners[6],corners[2]);
	segments[9]=Segment(corners[2],corners[3]);
	segments[10]=Segment(corners[2],corners[1]);
	segments[11]=Segment(corners[3],corners[7]);
}


void CubeGeometry::draw(){
	Vector3 hCL=length/2;
	Vector3 c=center;

	glPushMatrix();
	//these are the outlines of the cube
	glBegin( GL_QUADS);
	glColor3f(  1.0f,  1.0f, 0.0f );
	glVertex3f( c.x+hCL.x, c.y-hCL.y, c.z+hCL.z );
	glVertex3f( c.x+hCL.x,  c.y+hCL.y, c.z+hCL.z );
	glVertex3f( c.x-hCL.x,  c.y+hCL.y, c.z+hCL.z );
	glVertex3f( c.x-hCL.x, c.y-hCL.y, c.z+hCL.z );
	glEnd();

	// Purple side - RIGHT
	glBegin( GL_QUADS);
	glColor3f(  1.0f,  0.0f,  1.0f );
	glVertex3f( c.x+hCL.x, c.y-hCL.y, c.z-hCL.z );
	glVertex3f( c.x+hCL.x,  c.y+hCL.y, c.z-hCL.z );
	glVertex3f( c.x+hCL.x,  c.y+hCL.y,  c.z+hCL.z );
	glVertex3f( c.x+hCL.x, c.y-hCL.y,  c.z+hCL.z );

	// Green side - LEFT
	glColor3f(   0.0f,  1.0f,  0.0f );
	glVertex3f( c.x-hCL.x, c.y-hCL.y,  c.z+hCL.z );
	glVertex3f( c.x-hCL.x,  c.y+hCL.y,  c.z+hCL.z );
	glVertex3f( c.x-hCL.x,  c.y+hCL.y, c.z-hCL.z );
	glVertex3f( c.x-hCL.x, c.y-hCL.y, c.z-hCL.z );

	// Blue side - TOP
	glColor3f(   1.0f,  1.0f,  1.0f );
	glVertex3f( c.x+hCL.x,  c.y+hCL.y,  c.z+hCL.z );
	glVertex3f( c.x+hCL.x,  c.y+hCL.y, c.z-hCL.z );
	glVertex3f( c.x-hCL.x,  c.y+hCL.y, c.z-hCL.z );
	glVertex3f( c.x-hCL.x,  c.y+hCL.y,  c.z+hCL.z );

	// Red side - BOTTOM
	glColor3f(   1.0f,  0.0f,  0.0f );
	glVertex3f( c.x+hCL.x, c.y-hCL.y, c.z-hCL.z );
	glVertex3f( c.x+hCL.x, c.y-hCL.y,  c.z+hCL.z );
	glVertex3f( c.x-hCL.x, c.y-hCL.y,  c.z+hCL.z );
	glVertex3f( c.x-hCL.x, c.y-hCL.y, c.z-hCL.z );
	glEnd();

	glPopMatrix();
}


//i can't remember if i no longer use this function
Vector3 CubeGeometry::convertModelToTextureCoord (Vector3 modCoord) const{
	return Vector3((modCoord.x-center.x+halfLength.x)/length.x,(modCoord.y-center.y+halfLength.y)/length.y,(modCoord.z-center.z+halfLength.z)/length.z);
}









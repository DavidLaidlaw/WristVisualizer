#include "StdAfx.h"
#include "CubeGeometry.h"
#include <Windows.h>
#include <GL/glew.h>


CubeGeometry::CubeGeometry(Vector3 c, float l)
{
	center=c;
	length=l;//size of cube to be drawn
	halfLength=length/2;
	setupCornersAndSegments();
}

CubeGeometry::CubeGeometry()
{
	center=Vector3(0,0,0);
	length=2;//size of cube to be drawn
	halfLength=length/2;
	setupCornersAndSegments();
}

CubeGeometry::~CubeGeometry(void)
{
	center=Vector3(0,0,0);
	length=2;//size of cube to be drawn
	halfLength=length/2;
	setupCornersAndSegments();
}



void CubeGeometry::setupCornersAndSegments(){
	float hL=halfLength;
	Vector3 c=center;

	corners[0]=Vector3(c.x-hL,c.y-hL,c.z+hL);
	corners[1]=Vector3(c.x-hL,c.y+hL,c.z+hL);
	corners[2]=Vector3(c.x+hL,c.y+hL,c.z+hL);
	corners[3]=Vector3(c.x+hL,c.y-hL,c.z+hL);

	corners[4]=Vector3(c.x-hL,c.y-hL,c.z-hL);
	corners[5]=Vector3(c.x-hL,c.y+hL,c.z-hL);
	corners[6]=Vector3(c.x+hL,c.y+hL,c.z-hL);
	corners[7]=Vector3(c.x+hL,c.y-hL,c.z-hL);


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
	float hCL=length/2;
	Vector3 c=center;
	glPushMatrix();

	glPushMatrix();
	//these are the outlines of the cube

	//figure this out in terms of corners[]
	glBegin( GL_QUADS);
	glColor3f(   0.0f,  0.0f, 0.0f );
	glVertex3f(  c.x+hCL, c.y-hCL, c.z+hCL );
	glVertex3f(  c.x+hCL,  c.y+hCL, c.z+hCL );
	glVertex3f( c.x-hCL,  c.y+hCL, c.z+hCL );
	glVertex3f( c.x-hCL, c.y-hCL, c.z+hCL );
	glEnd();

	// Purple side - RIGHT
	glBegin( GL_QUADS);
	glColor3f(  1.0f,  0.0f,  1.0f );
	glVertex3f( c.x+hCL, c.y-hCL, c.z-hCL );
	glVertex3f( c.x+hCL,  c.y+hCL, c.z-hCL );
	glVertex3f( c.x+hCL,  c.y+hCL,  c.z+hCL );
	glVertex3f( c.x+hCL, c.y-hCL,  c.z+hCL );
	glEnd();

	// Green side - LEFT
	glBegin( GL_QUADS);
	glColor3f(   0.0f,  1.0f,  0.0f );
	glVertex3f( c.x-hCL, c.y-hCL,  c.z+hCL );
	glVertex3f( c.x-hCL,  c.y+hCL,  c.z+hCL );
	glVertex3f( c.x-hCL,  c.y+hCL, c.z-hCL );
	glVertex3f( c.x-hCL, c.y-hCL, c.z-hCL );
	glEnd();

	// Blue side - TOP
	glBegin( GL_QUADS);
	glColor3f(   0.0f,  0.0f,  1.0f );
	glVertex3f(  c.x+hCL,  c.y+hCL,  c.z+hCL );
	glVertex3f(  c.x+hCL,  c.y+hCL, c.z-hCL );
	glVertex3f( c.x-hCL,  c.y+hCL, c.z-hCL );
	glVertex3f( c.x-hCL,  c.y+hCL,  c.z+hCL );
	glEnd();

	// Red side - BOTTOM
	glBegin( GL_QUADS);
	glColor3f(   1.0f,  0.0f,  0.0f );
	glVertex3f(  c.x+hCL, c.y-hCL, c.z-hCL );
	glVertex3f(  c.x+hCL, c.y-hCL,  c.z+hCL );
	glVertex3f( c.x-hCL, c.y-hCL,  c.z+hCL );
	glVertex3f( c.x-hCL, c.y-hCL, c.z-hCL );
	glEnd();

	glPopMatrix();
}

Vector3 CubeGeometry::convertModelToTextureCoord (Vector3 modCoord) const{
	return Vector3((modCoord.x-center.x+halfLength)/length,(modCoord.y-center.y+halfLength)/length,(modCoord.z-center.z+halfLength)/length);
}









#include "StdAfx.h"
#include "Camera.h"

libCoin3D::Camera::Camera()
{
	_node = new SoOrthographicCamera();
}

void libCoin3D::Camera::rotateCameraInX(const float movement)
{
	this->rotateCamera(SbVec3f(-1, 0, 0), movement);
}

void libCoin3D::Camera::rotateCameraInY(const float movement)
{
	this->rotateCamera(SbVec3f(0, -1, 0), movement);
}

void libCoin3D::Camera::rotateCamera(const SbVec3f & aroundaxis, const float delta)
{
	SoCamera* cam = (SoOrthographicCamera*)_node;

	const SbVec3f DEFAULTDIRECTION(0, 0, -1);
	const SbRotation currentorientation = cam->orientation.getValue();

	SbVec3f currentdir;
	currentorientation.multVec(DEFAULTDIRECTION, currentdir);

	const SbVec3f focalpoint = cam->position.getValue() +
		cam->focalDistance.getValue() * currentdir;
	
	// set new orientation
	cam->orientation = SbRotation(aroundaxis, delta) * currentorientation;

	SbVec3f newdir;
	cam->orientation.getValue().multVec(DEFAULTDIRECTION, newdir);
	cam->position = focalpoint - cam->focalDistance.getValue() * newdir;
}
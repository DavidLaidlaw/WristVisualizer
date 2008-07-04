#include "StdAfx.h"
#include "Camera.h"

libCoin3D::Camera::Camera()
{
	_node = new SoOrthographicCamera();
}

libCoin3D::Camera::Camera(SoCamera *camera)
{
	_node = camera;
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
	SoCamera* cam = (SoCamera*)_node;

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

array<float>^ libCoin3D::Camera::getPosition()
{
	SoCamera* cam = (SoCamera*)_node;
	float x,y,z;
	cam->position.getValue().getValue(x,y,z);
	return gcnew array<float> {x, y, z};
}
array<float>^ libCoin3D::Camera::getOrientation()
{
	SoCamera* cam = (SoCamera*)_node;
	SbVec3f axis;
	float radians;
	cam->orientation.getValue().getValue(axis,radians);
	float x,y,z;
	axis.getValue(x,y,z);
	return gcnew array<float> {x, y, z, radians};
}
void libCoin3D::Camera::setPosition(array<float>^ position)
{
	if (position->Length != 3)
		throw gcnew System::ArgumentException("Position must have 3 values. (x, y, z)");
	SoCamera* cam = (SoCamera*)_node;
	cam->position.setValue(position[0],position[1],position[2]);
}
void libCoin3D::Camera::setOrientation(array<float>^ orientation)
{
	if (orientation->Length != 4)
		throw gcnew System::ArgumentException("Orientation must have 3 values. ((x, y, z), radians)");
	SoCamera* cam = (SoCamera*)_node;
	cam->orientation.setValue(SbRotation(SbVec3f(orientation[0],orientation[1],orientation[2]),orientation[3]));
}

float libCoin3D::Camera::FocalDistance::get() 
{
	SoCamera* cam = (SoCamera*)_node;
	float fd = cam->focalDistance.getValue();
	return fd;
}

void libCoin3D::Camera::FocalDistance::set(float value) 
{
	SoCamera* cam = (SoCamera*)_node;
	cam->focalDistance.setValue(value);
}
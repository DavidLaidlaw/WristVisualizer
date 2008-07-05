#include "StdAfx.h"
#include "Camera.h"

#include <Inventor/actions/SoSearchAction.h>

libCoin3D::Camera::Camera()
{
	_node = new SoOrthographicCamera();
	_node->ref();
}

libCoin3D::Camera::Camera(SoCamera *camera)
{
	_node = camera;
	_node->ref();
}

libCoin3D::Camera::Camera(System::String^ scenegraph)
{
	char* buffer = (char*)System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(scenegraph).ToPointer();
	SoInput in;
	in.setBuffer(buffer, scenegraph->Length + 1);
	SoNode* node = SoDB::readAll(&in);
	System::Runtime::InteropServices::Marshal::FreeHGlobal((System::IntPtr)buffer);
	if (node==NULL)
		throw gcnew System::ArgumentException("Error reading camera");

	//now lets try and find the camera
	node->ref();
	SoSearchAction sa;
	sa.setType(SoCamera::getClassTypeId());
	sa.apply(node);
	SoPath* myPath = sa.getPath();
	if (myPath==NULL) 
		throw gcnew System::ArgumentException("No camera found in graph");

	SoNode* camera = myPath->getTail();
	camera->ref();
	node->unref(); //delete wrapper junk, just save the camera
	_node = camera; //and now we can return :)

	//System::Console::WriteLine("Graph header of type: {0}", gcnew System::String(camera->getTypeId().getName().getString()));
}

libCoin3D::Camera::!Camera()
{
	if (_node != NULL)
		_node->unref();
}

bool libCoin3D::Camera::IsOrthographic::get()
{
	return (_node->getTypeId()==SoOrthographicCamera::getClassTypeId());
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

float libCoin3D::Camera::FarDistance::get() 
{
	SoCamera* cam = (SoCamera*)_node;
	float fd = cam->farDistance.getValue();
	return fd;
}

void libCoin3D::Camera::FarDistance::set(float value) 
{
	SoCamera* cam = (SoCamera*)_node;
	cam->farDistance.setValue(value);
}

float libCoin3D::Camera::NearDistance::get() 
{
	SoCamera* cam = (SoCamera*)_node;
	float nd = cam->nearDistance.getValue();
	return nd;
}

void libCoin3D::Camera::NearDistance::set(float value) 
{
	SoCamera* cam = (SoCamera*)_node;
	cam->nearDistance.setValue(value);
}

float libCoin3D::Camera::Height::get() 
{
	if (!this->IsOrthographic) return 0;
	SoOrthographicCamera* cam = (SoOrthographicCamera*)_node;
	float ht = cam->height.getValue();
	return ht;
}

void libCoin3D::Camera::Height::set(float value) 
{
	if (!this->IsOrthographic) return;
	SoOrthographicCamera* cam = (SoOrthographicCamera*)_node;
	cam->height.setValue(value);
}
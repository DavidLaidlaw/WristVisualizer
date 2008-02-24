#include "StdAfx.h"
#include "Sphere.h"

libCoin3D::Sphere::Sphere()
{
	_sphere = new SoSphere();
	_sphere->radius.setValue(3); //default radius size;
}

libCoin3D::Sphere::Sphere(float radius)
{
	_sphere = new SoSphere();
	_sphere->radius.setValue(radius);
}

SoNode* libCoin3D::Sphere::getNode()
{
	return _sphere;
}

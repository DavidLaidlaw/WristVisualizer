#include "StdAfx.h"
#include "Plane.h"


Plane::Plane(Vector3 n, Vector3 p)
{
	normal=n;
	point=p;
}


Plane::~Plane(void)
{
}

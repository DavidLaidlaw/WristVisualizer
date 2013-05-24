#pragma once
#include "Vector.h"

class Plane
{
public:
	Plane(Vector3 n, Vector3 p);
	virtual ~Plane(void);
	Vector3 getNormal(){
		return normal;
	}
	Vector3 getPoint(){
		return point;
	}

private:
	Vector3 normal;
	Vector3 point;
};


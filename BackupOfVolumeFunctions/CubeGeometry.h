#pragma once
#include "Vector.h"
#include "Segment.h"
#include <vector>

using namespace std;

class CubeGeometry
{
public:
	CubeGeometry(Vector3 c, float l);

	CubeGeometry();

	virtual ~CubeGeometry(void);

	Segment* getSegments(){
		return segments;
	}

	Vector3* getCorners(){
		return corners;
	}

	float getLength(){
		return length;
	}

	void draw();

	Vector3 convertModelToTextureCoord (Vector3 modCoord) const;

	void setupCornersAndSegments();

private:
	//8 corners
	Vector3 corners[8];

	//and some segments
	Segment segments[12];

	Vector3 center;
	float length;
	float halfLength;
};


#pragma once

#include "CubeGeometry.h"
#include "Vector.h"
#include <vector>
#include "Plane.h"

using namespace std;
class ProxyPiece
{
public:
	ProxyPiece(const Plane& p, vector<Vector3>& co,const CubeGeometry& c);
	virtual ~ProxyPiece(void);

	bool hasCorners();
	float comp(Vector3 c, Vector3 a, Vector3 b);
	void sortPointsInClockwiseOrderAroundAverageCenter();
	void draw();

private:
	CubeGeometry cube;
	vector<Vector3> corners;
	Plane plane;
};


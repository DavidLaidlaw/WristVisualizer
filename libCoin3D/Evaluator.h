#pragma once

#include <vector>
#include "Vector.h"
#include "ProxyPiece.h"
#include "Plane.h"
#include "CubeGeometry.h"
#include "Segment.h"

class Evaluator
{
public:
	Evaluator(void);
	virtual ~Evaluator(void);

	bool doesPlaneIntersectLineSegment(Vector3 start, Vector3 end, Vector3 normal, Vector3 point);

	Vector3 intersectPlaneAndLineSegment(Vector3 start, Vector3 end, Vector3 normal, Vector3 point);

	vector<Vector3> findAllIntersections(CubeGeometry cube, Plane p);

	ProxyPiece createSingleProxyPiece(CubeGeometry cube, Plane p);

	Vector3 findNearestCornerOnLine(CubeGeometry cube, Vector3 dir);

	Vector3 findFarthestCornerOnLine(CubeGeometry cube, Vector3 dir);

	vector<ProxyPiece> createProxyPieces(CubeGeometry cube, Vector3 viewDir, Vector3 startLoc, Vector3 endLoc, int numPieces);
};


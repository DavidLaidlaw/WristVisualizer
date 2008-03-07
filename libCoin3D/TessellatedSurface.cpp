#include "StdAfx.h"
#include "TessellatedSurface.h"

libCoin3D::TessellatedSurface::TessellatedSurface(SoCoordinate3* coordinates, SoIndexedFaceSet* indexedFaceSet)
{
	int numPts = coordinates->point.getNum();
	Points = gcnew array<array<float>^>(numPts);
	for (int i=0; i<numPts; i++) {
		Points[i] = gcnew array<float>(3); //always 3 points
		Points[i][0] = coordinates->point[i][0];
		Points[i][1] = coordinates->point[i][1];
		Points[i][2] = coordinates->point[i][2];
	}

	int numConn = indexedFaceSet->coordIndex.getNum();
	numConn = numConn/4;
	Connections = gcnew array<array<int>^>(numConn);
	for (int i=0; i<numConn; i++) {
		Connections[i] = gcnew array<int>(4); //always 4 connections
		Connections[i][0] = indexedFaceSet->coordIndex[i*4];
		Connections[i][1] = indexedFaceSet->coordIndex[i*4+1];
		Connections[i][2] = indexedFaceSet->coordIndex[i*4+2];
		Connections[i][3] = indexedFaceSet->coordIndex[i*4+3];
	}	
}

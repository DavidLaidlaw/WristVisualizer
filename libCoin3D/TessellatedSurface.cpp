#include "StdAfx.h"
#include "TessellatedSurface.h"

libCoin3D::TessellatedSurface::TessellatedSurface(SoCoordinate3* coordinates, SoIndexedFaceSet* indexedFaceSet)
{
	Points = TessellatedSurface::SoCoordinate3ToArray(coordinates);
	Connections = TessellatedSurface::SoIndexedFaceSetToArray(indexedFaceSet);
}


array<array<float>^>^ libCoin3D::TessellatedSurface::SoCoordinate3ToArray(SoCoordinate3* coordinates)
{
	int numPts = coordinates->point.getNum();
	array<array<float>^>^ points = gcnew array<array<float>^>(numPts);
	for (int i=0; i<numPts; i++) {
		points[i] = gcnew array<float>(3); //always 3 points
		points[i][0] = coordinates->point[i][0];
		points[i][1] = coordinates->point[i][1];
		points[i][2] = coordinates->point[i][2];
	}
	return points;
}

array<float,2>^ libCoin3D::TessellatedSurface::SoVertexProperyToArray(SoVertexProperty* vertex)
{
	int numPts = vertex->vertex.getNum();
	array<float,2>^ points = gcnew array<float,2>(numPts,3);
	for (int i=0; i<numPts; i++) {
		points[i,0] = vertex->vertex[i][0];
		points[i,1] = vertex->vertex[i][1];
		points[i,2] = vertex->vertex[i][2];
	}
	return points;
}

array<array<int>^>^ libCoin3D::TessellatedSurface::SoIndexedFaceSetToArray(SoIndexedFaceSet* indexedFaceSet)
{
	int numConn = indexedFaceSet->coordIndex.getNum();
	numConn = numConn/4;
	array<array<int>^>^ connections = gcnew array<array<int>^>(numConn);
	for (int i=0; i<numConn; i++) {
		connections[i] = gcnew array<int>(4); //always 4 connections
		connections[i][0] = indexedFaceSet->coordIndex[i*4];
		connections[i][1] = indexedFaceSet->coordIndex[i*4+1];
		connections[i][2] = indexedFaceSet->coordIndex[i*4+2];
		connections[i][3] = indexedFaceSet->coordIndex[i*4+3];
	}
	return connections;
}

array<int,2>^ libCoin3D::TessellatedSurface::SoIndexedFaceSetToMultiArray(SoIndexedFaceSet* indexedFaceSet)
{
	int numConn = indexedFaceSet->coordIndex.getNum();
	numConn = numConn/4;
	array<int,2>^ connections = gcnew array<int,2>(numConn,4);
	for (int i=0; i<numConn; i++) {
		connections[i,0] = indexedFaceSet->coordIndex[i*4];
		connections[i,1] = indexedFaceSet->coordIndex[i*4+1];
		connections[i,2] = indexedFaceSet->coordIndex[i*4+2];
		connections[i,3] = indexedFaceSet->coordIndex[i*4+3];
	}
	return connections;
}
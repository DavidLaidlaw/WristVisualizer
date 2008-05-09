#include "StdAfx.h"
#include "Contour.h"


libCoin3D::Contour::Contour(int numberOfDistances)
{
	_node = new SoSeparator();
	_node->ref();


	_drawStyle = new SoDrawStyle();
	_node->addChild(_drawStyle);
	_drawStyle->style = SoDrawStyle::LINES;
	_drawStyle->pointSize = 2.0;
	_drawStyle->lineWidth = 1.5;

	_pts = new SoCoordinate3();
	_lineSet = new SoLineSet();
	_node->addChild(_pts);
	_node->addChild(_lineSet);
	_pts->point.deleteValues(0);
	_lineSet->numVertices.deleteValues(0);

	_contourArea = gcnew array<double>(numberOfDistances);
	_contourCentroidSums = gcnew array<array<double>^>(numberOfDistances);
	for (int i=0; i<numberOfDistances; i++)
		_contourCentroidSums[i] = gcnew array<double>(3);
}


void libCoin3D::Contour::addLineSegment(float x0, float y0, float z0, float x1, float y1, float z1)
{
	int nextPtIndex = _pts->point.getNum();
	int nextLineIndex = _lineSet->numVertices.getNum();
	_pts->point.set1Value(nextPtIndex,x0,y0,z0);
	_pts->point.set1Value(nextPtIndex+1,x1,y1,z1);
	_lineSet->numVertices.set1Value(nextLineIndex,2);
}

void libCoin3D::Contour::setHidden(bool hidden)
{
	if (hidden) {
		_drawStyle->style = SoDrawStyle::INVISIBLE;
	}
	else {
		_drawStyle->style = SoDrawStyle::LINES;
	}
}

array<double>^ libCoin3D::Contour::Areas::get()
{
	return _contourArea;
}

void libCoin3D::Contour::Areas::set(array<double>^ value)
{
	_contourArea = value;
}

array<array<double>^>^ libCoin3D::Contour::CentroidSums::get()
{
	return _contourCentroidSums;
}

void libCoin3D::Contour::CentroidSums::set(array<array<double>^>^ value)
{
	_contourCentroidSums = value;
}

array<array<double>^>^ libCoin3D::Contour::Centroids::get()
{
	array<array<double>^>^ centroids = gcnew array<array<double>^>(_contourArea->Length);
	for (int i=0; i<centroids->Length; i++) {
		centroids[i] = gcnew array<double>(3);
		centroids[i][0] = _contourCentroidSums[i][0] / _contourArea[i];
		centroids[i][1] = _contourCentroidSums[i][1] / _contourArea[i];
		centroids[i][2] = _contourCentroidSums[i][2] / _contourArea[i];
	}
	return centroids;
}
#include "StdAfx.h"
#include "Contour.h"

libCoin3D::Contour::Contour(int numberOfDistances)
{
	array<System::Drawing::Color>^ contourColors = gcnew array<System::Drawing::Color>(numberOfDistances);
	for (int i=0; i<numberOfDistances; i++)
		contourColors[i] = System::Drawing::Color::White; //default to all white
	SetupContour(contourColors);
}

libCoin3D::Contour::Contour(array<System::Drawing::Color>^ contourColors)
{
	SetupContour(contourColors);
}

void libCoin3D::Contour::SetupContour(array<System::Drawing::Color>^ contourColors) {
	int numberOfDistances = contourColors->Length;

	_node = new SoSeparator();
	_node->ref();  //TODO: unref

	_drawStyle = new SoDrawStyle();
	_node->addChild(_drawStyle);
	_drawStyle->style = SoDrawStyle::LINES;
	_drawStyle->pointSize = 2.0;
	_drawStyle->lineWidth = 1.5;

	_contourSeparators = gcnew array<SoSeparator*>(numberOfDistances);
	_contourPts = gcnew array<SoCoordinate3*>(numberOfDistances);
	_contourLineSets = gcnew array<SoLineSet*>(numberOfDistances);
	_contourColors = gcnew array<SoBaseColor*>(numberOfDistances);

	_contourArea = gcnew array<double>(numberOfDistances);
	_contourCentroidSums = gcnew array<array<double>^>(numberOfDistances);
	for (int i=0; i<numberOfDistances; i++) {
		_contourCentroidSums[i] = gcnew array<double>(3);

		_contourSeparators[i] = new SoSeparator();
		_node->addChild(_contourSeparators[i]);
		_contourColors[i] = new SoBaseColor();
		_contourPts[i] = new SoCoordinate3();
		_contourLineSets[i] = new SoLineSet();
		_contourSeparators[i]->addChild(_contourColors[i]);
		_contourSeparators[i]->addChild(_contourPts[i]);
		_contourSeparators[i]->addChild(_contourLineSets[i]);
		_contourPts[i]->point.deleteValues(0);
		_contourLineSets[i]->numVertices.deleteValues(0);
		setColor(i, contourColors[i]);
	}
}


void libCoin3D::Contour::addLineSegment(float x0, float y0, float z0, float x1, float y1, float z1)
{
	int contourIndex = 0; //default to the first contour
	addLineSegment(contourIndex, x0, y0, z0, x1, y1, z1);
}

void libCoin3D::Contour::addLineSegment(int contourIndex, float x0, float y0, float z0, float x1, float y1, float z1)
{
	int nextPtIndex = _contourPts[contourIndex]->point.getNum();
	int nextLineIndex = _contourLineSets[contourIndex]->numVertices.getNum();
	_contourPts[contourIndex]->point.set1Value(nextPtIndex,x0,y0,z0);
	_contourPts[contourIndex]->point.set1Value(nextPtIndex+1,x1,y1,z1);
	_contourLineSets[contourIndex]->numVertices.set1Value(nextLineIndex,2);
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

void libCoin3D::Contour::setColor(int contourIndex, System::Drawing::Color color)
{
	_contourColors[contourIndex]->rgb.setValue(color.R/255.0f, color.G/255.0f, color.B/255.0f);
}
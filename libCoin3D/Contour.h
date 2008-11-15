#pragma once

#include "Node.h"
#include <Inventor/nodes/SoSeparator.h>
#include <Inventor/nodes/SoLineSet.h>
#include <Inventor/nodes/SoCoordinate3.h>
#include <Inventor/nodes/SoDrawStyle.h>
#include <Inventor/nodes/SoBaseColor.h>

namespace libCoin3D {
public ref class Contour : Node
{
public:
	Contour(int numberOfDistances);
	Contour(array<System::Drawing::Color>^ contourColors);
	virtual void addLineSegment(int contourIndex, float x0, float y0, float z0, float x1, float y1, float z1);
	virtual void addLineSegment(float x0, float y0, float z0, float x1, float y1, float z1);
	virtual void setHidden(bool hidden);

	property array<double>^ Areas {
		array<double>^ get();
		void set(array<double>^ value);
	}
	property array<array<double>^>^ CentroidSums {
		array<array<double>^>^ get();
		void set(array<array<double>^>^ value);
	}
	property array<array<double>^>^ Centroids {
		array<array<double>^>^ get();
	}

	virtual SoNode* getNode() override { return _node; }
private:
	virtual void SetupContour(array<System::Drawing::Color>^ contourColors);

	SoSeparator* _node;
	array<SoSeparator*>^ _contourSeparators;
	array<SoCoordinate3*>^ _contourPts;
	array<SoLineSet*>^ _contourLineSets;
	array<SoBaseColor*>^ _contourColors;

	SoDrawStyle* _drawStyle;
	array<double>^ _contourArea;
	array<array<double>^>^ _contourCentroidSums;

	void setColor(int contourIndex, System::Drawing::Color color);
};
}
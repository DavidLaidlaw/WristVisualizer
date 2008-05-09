#pragma once

#include "Node.h"
#include <Inventor/nodes/SoSeparator.h>
#include <Inventor/nodes/SoLineSet.h>
#include <Inventor/nodes/SoCoordinate3.h>
#include <Inventor/nodes/SoDrawStyle.h>

namespace libCoin3D {
public ref class Contour : Node
{
public:
	Contour(int numberOfDistances);
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
	SoSeparator* _node;
	SoCoordinate3* _pts;
	SoLineSet* _lineSet;
	SoDrawStyle* _drawStyle;
	array<double>^ _contourArea;
	array<array<double>^>^ _contourCentroidSums;
};
}
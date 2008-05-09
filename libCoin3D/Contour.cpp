#include "StdAfx.h"
#include "Contour.h"


libCoin3D::Contour::Contour()
{
	_node = new SoSeparator();
	_node->ref();


	_drawStyle = new SoDrawStyle();
	_node->addChild(_drawStyle);
	_drawStyle->style = SoDrawStyle::LINES;
	_drawStyle->pointSize = 4.0;
	_drawStyle->lineWidth = 2.0;

	_pts = new SoCoordinate3();
	_lineSet = new SoLineSet();
	_node->addChild(_pts);
	_node->addChild(_lineSet);
	_pts->point.deleteValues(0);
	_lineSet->numVertices.deleteValues(0);

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
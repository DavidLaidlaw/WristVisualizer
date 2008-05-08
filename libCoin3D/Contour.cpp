#include "StdAfx.h"
#include "Contour.h"

#include <Inventor/nodes/SoDrawStyle.h>


libCoin3D::Contour::Contour()
{
	_node = new SoSeparator();
	_node->ref();


	SoDrawStyle* draw = new SoDrawStyle();
	_node->addChild(draw);
	draw->style = SoDrawStyle::LINES;
	draw->pointSize = 4.0;
	draw->lineWidth = 2.0;

	_pts = new SoCoordinate3();
	_lineSet = new SoLineSet();
	_node->addChild(_pts);
	_node->addChild(_lineSet);

}


void libCoin3D::Contour::addLineSegment(float x0, float y0, float z0, float x1, float y1, float z1)
{
	int nextPtIndex = _pts->point.getNum();
	int nextLineIndex = _lineSet->numVertices.getNum();
	_pts->point.set1Value(nextPtIndex,x0,y0,z0);
	_pts->point.set1Value(nextPtIndex+1,x1,y1,z1);
	_lineSet->numVertices.set1Value(nextLineIndex,2);
}
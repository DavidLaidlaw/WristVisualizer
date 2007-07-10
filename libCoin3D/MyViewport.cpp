#include "StdAfx.h"
#include "MyViewport.h"

#include <Inventor/SbViewportRegion.h>
#include <Inventor/nodes/SoSphere.h>


MyViewport::MyViewport() : SoSeparator()
{
	this->addChild(new SoSphere);
}

MyViewport::~MyViewport()
{
}

void MyViewport::GLRenderBelowPath(SoGLRenderAction *action)
{
	const SbViewportRegion currentRegion = action->getViewportRegion();
	SbViewportRegion newRegion;
	newRegion.setViewportPixels(5,5,50,50);
	action->setViewportRegion(newRegion);
	SoSeparator::GLRenderBelowPath(action);
	action->setViewportRegion(currentRegion);
}
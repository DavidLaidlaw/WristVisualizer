#pragma once
#include <Inventor/nodes/SoSeparator.h>
#include <Inventor/actions/SoGLRenderAction.h>

class MyViewport :
	public SoSeparator
{
public:
	MyViewport();
	virtual void GLRenderBelowPath(SoGLRenderAction* action);
public:
	~MyViewport();
};

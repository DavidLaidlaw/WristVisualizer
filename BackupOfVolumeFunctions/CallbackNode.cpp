#include "StdAfx.h"
#include "CallbackNode.h"
#include <Inventor\actions\SoGLRenderAction.h>
#include <Inventor\elements\SoCacheElement.h>


libCoin3D::CallbackNode::CallbackNode()
{
	//the node
	volCallback=new SoCallback();
	volCallback->ref();
	mCube=new MasterCube();

}


libCoin3D::CallbackNode::~CallbackNode()
{
	volCallback->unref();
	delete mCube;
}


//open gl rendering callback
void myGLCallback(void *myData, SoAction *action)
{
	if (action->isOfType(SoGLRenderAction::getClassTypeId())){
		SoCacheElement::invalidate(action->getState());//disables caching so it is always called
		//printf("call back working\n");
		MasterCube* mc=(MasterCube*) myData;
		mc->renderCube();
	}
}


void libCoin3D::CallbackNode::setUpCallBack(){
	//pass in the master cube as data 
	//so it can be accessed in the render callback
	volCallback->setCallback(myGLCallback,mCube);
}

//returns the node of it
SoNode* libCoin3D::CallbackNode::getNode()
{
	return volCallback;
}

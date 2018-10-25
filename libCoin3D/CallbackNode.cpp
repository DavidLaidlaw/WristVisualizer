#include "StdAfx.h"
#include "CallbackNode.h"
#include <Inventor\actions\SoGLRenderAction.h>
#include <Inventor\actions\SoPickAction.h>
#include <Inventor\actions\SoHandleEventAction.h>
#include <Inventor\elements\SoCacheElement.h>


libCoin3D::CallbackNode::CallbackNode(int w ,int h, int l,int x, int y, int z, cli::array<int>^ d, bool IsLeft)
{
	//the node
	volCallback=new SoCallback();
	volCallback->ref();

	//for(int i=0;i<d->Length;i++){
	//	if(d[i]>100){printf("some data %i \n",d[i]);fflush(stdout);}
	//}
	isLeft=IsLeft;
	cli::pin_ptr<int> pArrayElement = &d[0];
	mCube=new MasterCube(w,h,l,x,y,z,pArrayElement,isLeft);
}


libCoin3D::CallbackNode::~CallbackNode()
{
	volCallback->unref();
	delete mCube;
}


//open gl rendering callback
void myGLCallback(void *myData, SoAction *action)
{
		MasterCube* mc=(MasterCube*) myData;
		//next track press and release of events

		if (action->isOfType(SoGLRenderAction::getClassTypeId())){
			//printf("call back working\n");
			SoCacheElement::invalidate(action->getState());
			mc->renderCube();
		}
	

}

void libCoin3D::CallbackNode::setDoDrawVolume(bool b){
	mCube->setDoDrawVolume(b);
}

void libCoin3D::CallbackNode::setSliceNum(int num){
		mCube->setSliceNum(num);
}

void libCoin3D::CallbackNode::SetIsOpaque(bool b){
	mCube->setIsOpaque(b);
}

void libCoin3D::CallbackNode::setOpacity(float o){
	mCube->setOpacity(o);
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

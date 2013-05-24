#include "StdAfx.h"
#include "CenterballDragger.h"
#include <Inventor\nodes\SoCube.h>
#include <math.h>



libCoin3D::CenterballDragger::CenterballDragger()
{
	_centerBall = new SoCenterballDragger();
	_centerBall->ref();
	_centerBallManip=new SoCenterballManip();
	_centerBallManip->ref();
	_centerBallManip->ref();

	CBenabled=true;

	currCentX=0;
	currCentY=0;
	currCentZ=0;

	currRotX=0;
	currRotY=0;
	currRotZ=0;

	currTx=0;
	currTy=0;
	currTz=0;

	float b0,b1,b2,b3;
	this->getManip()->rotation.getValue().getValue(b0,b1,b2,b3);
	bq0=b0;
	bq1=b1;
	bq2=b2;
	bq3=b3;
}


libCoin3D::CenterballDragger::~CenterballDragger()
{
	_centerBall->unref();
	_centerBallManip->unref();
}

libCoin3D::CenterballDragger::CenterballDragger(SoCenterballDragger* centerballDragger)
{
	_centerBall = centerballDragger;
	if (_centerBall==NULL)
		_centerBall = new SoCenterballDragger();
}

libCoin3D::CenterballDragger::CenterballDragger(SoCenterballManip* centerballManip)
{
	_centerBallManip = centerballManip;
	if (_centerBallManip==NULL)
		_centerBallManip = new SoCenterballManip();
}


void libCoin3D::CenterballDragger::setTranslation(float x, float y, float z){
	_centerBallManip->translation.setValue(x,y,z);
	//currTx=x;
	//currTy=y;
	//currTz=z;
		currCentX=x;
	currCentY=y;
	currCentZ=z;
}

void libCoin3D::CenterballDragger::setCenter(float x, float y, float z){
	currCentX=x;
	currCentY=y;
	currCentZ=z;
	_centerBallManip->center.setValue(x,y,z);
	//_centerBallManip->recenter(SbVec3f(x,y,z));
}

void libCoin3D::CenterballDragger::setRotation(float x, float y, float z){
	//if(x==0 && y==0 && z==0){
	//	this->getManip()->rotation.setValue(SbVec3f(1,1,1),0);
	//}
	//else{
	//	float rx=z;
	//	float ry=-y;
	//	float rz=-x;
	//	float q0=cos(rx/2)*cos(ry/2)*cos(rz/2)+sin(rx/2)*sin(ry/2)*sin(rz/2);
	//	float q1=sin(rx/2)*cos(ry/2)*cos(rz/2)-cos(rx/2)*sin(ry/2)*sin(rz/2);
	//	float q2=cos(rx/2)*sin(ry/2)*cos(rz/2)+sin(rx/2)*cos(ry/2)*sin(rz/2);
	//	float q3=cos(rx/2)*cos(ry/2)*sin(rz/2)-sin(rx/2)*sin(ry/2)*cos(rz/2);
	//	this->getManip()->rotation.setValue(SbRotation(q0,q1,q2,q3));
	//}

	CBenabled=false;
	SbRotation rotx,roty,rotz;  
	SbMatrix   mtx,mty,mtz;

	float xr=x;
	float yr=y;
	float zr=z;
	//printf("current rot change %f  %f  %f\n",xr,yr,zr);
	rotz.setValue(SbVec3f(1, 0, 0), (xr));
	roty.setValue(SbVec3f(0, 1, 0), (yr));
	rotx.setValue(SbVec3f(0, 0, 1), (zr));    

	rotx.getValue(mtx);
	roty.getValue(mty);    
	rotz.getValue(mtz);

	this->getManip()->multRight(mtx);
	this->getManip()->multRight(mty);
	this->getManip()->multRight(mtz);
	CBenabled=true;


}

delegate void tempFunc();

void libCoin3D::CenterballDragger::memberCallbackFromText(float tx, float ty, float tz, float rx, float ry, float rz, bool doTranslate){
	CBenabled=false;
	//move the manipulator, but do not use CB or call to C#


	//printf("current center %f  %f  %f\n",currCentX,currCentY,currCentZ);
	//this->getManip()->rotation.setValue(SbVec3f(0,0,0),0);
	//this->getManip()->set("transform { translation 0 0 1 }");
	//this->setCenter(tx,ty,tz);
	//this->getManip()->center.setValue(tx,ty,tz);
	//float x,y,z;
	//this->getManip()->translation.getValue().getValue(x,y,z);
	//float u,v,w;
	//this->getManip()->translation.getValue().getValue(u,v,w);
	//x+=u;y+=v;z+=w;

	//this->setCenter(0,0,0);
	//	if((tx!=0 || ty!=0 || tz!=0)){
	//	//this->setTranslation((tx-x)/1,(ty-y)/1,(tz-z)/1);
	//	
	//}

	this->setRotation(rx,ry,rz);
	this->setCenter((tx),(ty),(tz));

	//this->getManip()->recenter(SbVec3f(tx,ty,tz));
	//printf("center t %f  %f  %f\n",x,y,z);
	//printf("current translation change just t %f  %f  %f\n",tx,ty,tz);
	//printf("current translation change %f  %f  %f\n",tx-x,ty-y,tz-z);
	//maybe will need to temporarily reeanble tranalstion

	////create a new rotation value and give it to the centerball
	//SbVec3f* rotVec=new SbVec3f(rx,ry,rz);
	//float r=rotVec->length();
	//r=rotVec->normalize();

	////this->getManip()->rotation.setValue(SbVec3f(rx,ry,rz),0);
	////this->getManip()->rotation.setValue(SbVec3f(rx,ry,rz),1);

	//SbVec3f* rotangle=new SbVec3f(rx,ry,rz);
	//float ux,uy,uz;
	//float lengthbefore=rotangle->length();
	//r=rotangle->normalize();

	//rotangle->getValue(ux,uy,uz);
	//float lengthafter=rotangle->length();

	//printf ("rotation axis: %f %f %f radians: %f  length before: %f  after: %f\n", ux,uy,uz,r, lengthbefore, lengthafter);
	//fflush(stdout);

	//if(ux==0 && uy==0 && uz==0){
	//	this->getManip()->rotation.setValue(SbVec3f(1,1,1),0);
	//}
	//else{
	//	this->getManip()->rotation.setValue(SbVec3f(ux,uy,uz),r);
	//	fflush(stdout);
	//}


	//FIX TRANSLATION INPUT OF THIS METHOD TO BE DELTA AND NOT ABSOLUTE
	//find how much center moved
	//float deltaX=tx-currCentX;
	//float deltaY=ty-currCentY;
	//float deltaZ=tz-currCentZ;
	//currCentX=tx;
	//currCentY=ty;
	//currCentZ=tz;

	CBenabled=true;
}


void libCoin3D::CenterballDragger::memberCallback(SoCenterballManip* dragger){
	if(CBenabled){
		fflush(stdout);
		float q0, q1, q2, q3;
		float cx,cy,cz;

		this->getManip()->rotation.getValue().getValue(q0,q1,q2,q3);

		this->getManip()->center.getValue().getValue(cx,cy,cz);

		//printf("cent %f  %f %f \n",cx,cy,cz);
		//float f=37.0f;
		//if(cx>f)this->getManip()->center.setValue(f,cy,cz);
		//else if(cx<-f)this->getManip()->center.setValue(-f,cy,cz);

		//if(cy>f)this->getManip()->center.setValue(cx,f,cz);
		//else if(cy<-f)this->getManip()->center.setValue(cx,-f,cz);

		//if(cz>f)this->getManip()->center.setValue(cx,cy,f);
		//else if(cz<-f)this->getManip()->center.setValue(cx,cy,-f);

		//this->getManip()->center.getValue().getValue(cx,cy,cz);

		//find how much center moved
		float deltaX=cx-(currCentX);
		float deltaY=cy-(currCentY);
		float deltaZ=cz-(currCentZ);


		currCentX=cx;
		currCentY=cy;
		currCentZ=cz;
		//printf("current center %f  %f  %f\n",currCentX,currCentY,currCentZ);

		_CSharpCallBack(deltaX,deltaY,deltaZ,q0,q1,q2,q3);
	}
}


delegate void tempCB( void * userData, SoDragger *dragger);

//THIS is the callback that the libcoin3d manip will accept
//it shall call the stored function pointer to the CSharp function
void intermediateCallback( void * userData, SoDragger *dragger)
{
	using System::Runtime::InteropServices::GCHandle;
	GCHandle gch = GCHandle::FromIntPtr((System::IntPtr)userData);
	libCoin3D::CenterballDragger^ cbd= (libCoin3D::CenterballDragger^)gch.Target;

	float q0, q1, q2, q3;
	cbd->getManip()->rotation.getValue().getValue(q0,q1,q2,q3);
	fflush(stdout);

	cbd->memberCallback((SoCenterballManip*)dragger);
	//keep the pointer and delete it later otherwise everything gets ALL SCREWED UPPPPPP
	// gch.Free(); // If the call only once, otherwise it is necessary to keep the pointer and delete it later     
}



//this is TERRIBLE
void libCoin3D::CenterballDragger::addCB(delFunc^ f){
	_CSharpCallBack=f;

	using System::Runtime::InteropServices::GCHandle;
	void* void_this=(void*)GCHandle::ToIntPtr(GCHandle::Alloc(this));


	_centerBallManip->getDragger()->addValueChangedCallback(intermediateCallback, void_this);

}



void libCoin3D::CenterballDragger::reactToValueChanged(){
	//SoCube myCube*=new SoCube;

}



void libCoin3D::CenterballDragger::dragStart()
{
}

void libCoin3D::CenterballDragger::drag()
{
}

void libCoin3D::CenterballDragger::dragFinish()
{
}

void libCoin3D::CenterballDragger::setAnimationEnabled(bool newVal)
{
}

bool libCoin3D::CenterballDragger::isAnimationEnabled()
{
	return false;
}


SoNode* libCoin3D::CenterballDragger::getNode()
{
	return _centerBallManip;
}


SoCenterballManip* libCoin3D::CenterballDragger::getManip()
{
	return _centerBallManip;
}

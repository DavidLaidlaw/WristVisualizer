#pragma once

#include "Node.h"
#include <Inventor/nodes/SoNode.h>
#include <Inventor/draggers/SoCenterballDragger.h>
#include <Inventor\manips\SoCenterballManip.h>


namespace libCoin3D {
public ref class CenterballDragger : Node
{
public:
	CenterballDragger();
	~CenterballDragger();
	CenterballDragger(SoCenterballDragger* centerballDragger);
	CenterballDragger(SoCenterballManip* centerBallManip);
	void setTranslation(float x, float y, float z);
	void setCenter(float x, float y, float z);
	void setRotation(float x, float y, float z);
	void reactToValueChanged();

	virtual void dragStart();
	virtual void drag();
	virtual void dragFinish();
	virtual void setAnimationEnabled(bool newVal);
	virtual bool isAnimationEnabled();
	virtual SoNode* getNode() override;
	virtual SoCenterballManip* getManip();

	//void intermediateCallback( void * userData, SoDragger *dragger);

	delegate void delFunc(float,float,float,float,float,float,float);
	void addCB(delFunc^ f);
	void memberCallback(SoCenterballManip* centerBallManip);
	void memberCallbackFromText(float tx, float ty, float tz, float rx, float ry, float rz, bool doTranslate);

	void setCBEnabled(bool b){
		CBenabled=b;
	}

private: 
	SoCenterballDragger* _centerBall;
	SoCenterballManip* _centerBallManip;
	static delFunc^ _CSharpCallBack;
	bool CBenabled;

	float currCentX;
	float currCentY;
	float currCentZ;
	
	float currRotX;
	float currRotY;
	float currRotZ;

	float currTx;
	float currTy;
	float currTz;
	
	float bq0,bq1,bq2,bq3;

};
}


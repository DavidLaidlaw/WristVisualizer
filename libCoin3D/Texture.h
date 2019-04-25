#pragma once

#include <Inventor/nodes/SoTexture2.h>
#include <Inventor/draggers/SoTranslate1Dragger.h>

#include "CenterballDragger.h"
#include <Inventor\draggers\SoCenterballDragger.h>
#include <Inventor\manips\SoCenterballManip.h>


#include "Separator.h"

void updateTextureCB( void * data, int positionValue, SoSensor * );
class TextureCBData;

namespace libCoin3D {
public ref class Texture
{
public:
	enum struct Sides { LEFT, RIGHT };
	enum struct Planes { XY_PLANE, YZ_PLANE };

	Texture(Sides side, int sizeX, int sizeY, int sizeZ, double voxelX, double voxelY, double voxelZ);
	!Texture();

	virtual Separator^ makeDragerAndTexture(array<array<System::Byte>^>^ data, Planes plane);
	virtual Separator^ makeDragerAndTexture(array<array<System::Byte>^>^ data, int dragPos, Planes plane);

	array<array<System::Byte>^> ^voxels;
	//for drawing the volume render stuff
	//void myGLCallback(void *myData, SoAction *action);

	///////////////////
	void makeCenterballManips(Separator^ bone);
	/////////////////////

	static Separator^ createPointsFileObject(array<array<double>^>^ points, array<float>^ color);
	static Separator^ createPointsFileObject(array<array<double>^>^ points, float colorR, float colorG, float colorB);
	static Separator^ createPointsFileObject(array<array<double>^>^ points);

	//methods called from input events
	virtual Separator^ createKeyboardCallbackObject(int viewerParrentHWND);
	virtual void moveDragger(Planes plane,int howFar);
	virtual void moveDraggerXY(int howFar);
	virtual void moveDraggerYZ(int howFar);
	//virtual void moveCenterballTracker(int number);

	//static members, keeping track of all global ExaminerViewers
	static System::Collections::Hashtable^ TexturesHashtable = gcnew System::Collections::Hashtable();
	static Texture^ getTextureByParentWidget(int HWND);
 public:
	SoTranslate1Dragger* get_draggerXY(){ return  _draggerXY;}
	SoTranslate1Dragger* get_draggerYZ(){ return  _draggerYZ;}

	int getNumSlicesYZ();
	int getNumSlicesXY();

	delegate void delFunc(int);
	void addCBtrack1(delFunc^ f);
	void addCBtrack2(delFunc^ f);
	static delFunc^ _CSharpTrack1CB;
	static delFunc^ _CSharpTrack2CB;
	//set centerball stuff
private:
	unsigned char** allocateSliceStack(int numPixelsX, int numPixelsY, int numPixelsZ);
	SoSeparator* makeRectangle(Planes plane);

	unsigned char** setupLocalBuffer(array<array<System::Byte>^>^ data, Planes plane);
 public:
	Sides _side;
	int _sizeX, _sizeY, _sizeZ;
	double _voxelX, _voxelY, _voxelZ;
	short* _data;
	unsigned char** _all_slice_dataXY;
	unsigned char** _all_slice_dataYZ;
	// voxels used in the setup of the translatedraggers
	unsigned char** bufferXY;
	unsigned char** bufferYZ;
	TextureCBData * textureCBdataXY;
	TextureCBData * textureCBdataYZ;
private:
	//SoCenterballDragger* _centerBall;
	//SoCenterballManip* _centerBallManip;
	//CenterballDragger^ _centerballDragger;

	CenterballDragger^ _centerballDragger0;
	CenterballDragger^ _centerballDragger1;
	CenterballDragger^ _centerballDragger2;
public:
	SoTranslate1Dragger* _draggerXY;
	SoTranslate1Dragger* _draggerYZ;

};
}

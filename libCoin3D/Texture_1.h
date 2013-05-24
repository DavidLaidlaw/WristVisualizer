#pragma once

#include <Inventor/nodes/SoTexture2.h>
#include <Inventor/draggers/SoTranslate1Dragger.h>

#include "Separator.h"

namespace libCoin3D {
public ref class Texture
{
public:
	enum struct Sides { LEFT, RIGHT };
	enum struct Planes { XY_PLANE, YZ_PLANE };

	Texture(Sides side, int sizeX, int sizeY, int sizeZ, double voxelX, double voxelY, double voxelZ);
	!Texture();

	virtual Separator^ makeDragerAndTexture(array<array<System::Byte>^>^ data, Planes plane);

	static Separator^ createPointsFileObject(array<array<double>^>^ points, array<float>^ color);
	static Separator^ createPointsFileObject(array<array<double>^>^ points, float colorR, float colorG, float colorB);
	static Separator^ createPointsFileObject(array<array<double>^>^ points);

	virtual Separator^ createKeyboardCallbackObject(int viewerParrentHWND);
	virtual void moveDragger(Planes plane,int howFar);

	//static members, keeping track of all global ExaminerViewers
	static System::Collections::Hashtable^ TexturesHashtable = gcnew System::Collections::Hashtable();
	static Texture^ getTextureByParentWidget(int HWND);
private:
	unsigned char** allocateSliceStack(int numPixelsX, int numPixelsY, int numPixelsZ);
	SoSeparator* makeRectangle(Planes plane);

	unsigned char** setupLocalBuffer(array<array<System::Byte>^>^ data, Planes plane);

	Sides _side;
	int _sizeX, _sizeY, _sizeZ;
	double _voxelX, _voxelY, _voxelZ;
	short* _data;
	unsigned char** _all_slice_dataXY;
	unsigned char** _all_slice_dataYZ;
	SoTranslate1Dragger* _draggerXY;
	SoTranslate1Dragger* _draggerYZ;
};
}

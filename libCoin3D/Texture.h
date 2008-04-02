#pragma once

#include <Inventor/nodes/SoTexture2.h>

#include "Separator.h"

namespace libCoin3D{
public ref class Texture
{
public:
	Texture();
	virtual ~Texture();
	static Separator^ createPointsFileObject(array<array<double>^>^ points);

	virtual Separator^ makeDragerAndTexture(array<array<System::Byte>^>^ data, int axis);
private:
	unsigned char** allocateSliceStack(int numPixelsX, int numPixelsY, int numPixelsZ);
	float** makeRectangleVertices();
	SoSeparator* makeRectangle(int axis);
	void setTextureZplane(SoTexture2* texture, unsigned char** all_slice_data);

	int _sizeX, _sizeY, _sizeZ;
	double _voxelX, _voxelY, _voxelZ;
	float** _verticesRectangle1;
	short* _data;
	unsigned char** _all_slice_data1;
	unsigned char* _current_slice1;
	
};
}

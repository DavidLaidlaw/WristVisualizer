#pragma once

#include <Inventor/nodes/SoCoordinate3.h>
#include <Inventor/nodes/SoIndexedFaceSet.h>

namespace libCoin3D {
public ref class TessellatedSurface
{
public:
	TessellatedSurface(SoCoordinate3* coordinates, SoIndexedFaceSet* indexedFaceSet);
	int TestVar;
	array<array<float>^>^ Points;
	array<array<int>^>^ Connections;
private:
	//convertToArray(SoCoordinate3* coordinates
};
}

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

	static array<array<float>^>^ SoCoordinate3ToArray(SoCoordinate3* coordinates);	
	static array<array<int>^>^ SoIndexedFaceSetToArray(SoIndexedFaceSet* indexedFaceSet);
	static array<float,2>^ SoVertexProperyToArray(SoVertexProperty* vertex);
	static array<int,2>^ SoIndexedFaceSetToMultiArray(SoIndexedFaceSet* indexedFaceSet);
private:
	//convertToArray(SoCoordinate3* coordinates
};
}

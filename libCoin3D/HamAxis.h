#pragma once
#include "Node.h"
#include <Inventor/nodes/SoNode.h>
#include <Inventor/nodes/SoSeparator.h>

namespace libCoin3D {
public ref class HamAxis : Node
{
public:
	HamAxis(float Nx, float Ny, float Nz, float Qx, float Qy, float Qz);
	HamAxis(double Nx, double Ny, double Nz, double Qx, double Qy, double Qz);
	virtual SoNode* getNode() override { return _node; }
private:
	virtual void makeHam(float Nx, float Ny, float Nz, float Qx, float Qy, float Qz);
	SoNode* _node;
};
}

#pragma once
#include "Node.h"
#include "Material.h"
#include <Inventor/nodes/SoNode.h>
#include <Inventor/nodes/SoSeparator.h>

namespace libCoin3D {
public ref class HamAxis : Node
{
public:
	HamAxis(float Nx, float Ny, float Nz, float Qx, float Qy, float Qz);
	HamAxis(double Nx, double Ny, double Nz, double Qx, double Qy, double Qz);
	HamAxis(Material^ m, double Nx, double Ny, double Nz, double Qx, double Qy, double Qz);
	void setColor(float r, float g, float b);
	property bool IsValid {
		bool get() { return _node!=NULL; }
	}
	virtual SoNode* getNode() override { return _node; }
private:
	void makeHam(float Nx, float Ny, float Nz, float Qx, float Qy, float Qz, SoMaterial* m);
	SoNode* _node;
};
}

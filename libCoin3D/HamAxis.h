#pragma once
#include "Node.h"
#include "Material.h"
#include <Inventor/nodes/SoNode.h>
#include <Inventor/nodes/SoSeparator.h>
#include <Inventor/nodes/SoCylinder.h>
#include <Inventor/nodes/SoCone.h>
#include <Inventor/nodes/SoTransform.h>

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

	virtual void SetHamDimensions(float length, float radius);
	virtual void SetHamLength(float length);
	virtual void SetHamRadius(float radius);

	virtual SoNode* getNode() override { return _node; }

private:
	void makeHam(float Nx, float Ny, float Nz, float Qx, float Qy, float Qz, SoMaterial* m);

	SoNode* _node;
	SoCylinder* _cylinder;
	SoCone* _cone;
	SoTransform* _coneTransform;

	static float AXIS_CONE_RADIUS_RATIO = 0.5;
	static float AXIS_CONE_LENGTH_RATIO = 15;
};
}

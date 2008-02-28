#pragma once

#include <Inventor/nodes/SoTransform.h>
#include <Inventor/SbLinear.h>

#include "Node.h"

namespace libCoin3D {
public ref class Transform : Node
{
public:
	Transform(void);
	~Transform();
	virtual void setRotation(double r00,double r01,double r02,double r10,double r11,double r12,double r20,double r21,double r22);
	virtual void setTranslation(double v0, double v1, double v2);
	virtual void setTransform(double r00,double r01,double r02,double r10,double r11,double r12,double r20,double r21,double r22,double v0, double v1, double v2);
	void invert();
	bool isIdentity();

	SoTransform* getSoTransform() { return _transform; }
	virtual SoNode* getNode() override;
	
private:
	SoTransform* _transform;
	SbMatrix* _rotMatrix;
};
}

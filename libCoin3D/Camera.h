#pragma once

#include "Node.h"
#include <Inventor/nodes/SoSeparator.h>
#include <Inventor/nodes/SoOrthographicCamera.h>


namespace libCoin3D {
public ref class Camera : Node
{
public:
	Camera();
	Camera(SoCamera* camera);
	virtual SoNode* getNode() override { return _node; }

	void rotateCameraInX(const float movement);
	void rotateCameraInY(const float movement);

	array<float>^ getPosition();
	array<float>^ getOrientation();
	void setPosition(array<float>^ position);
	void setOrientation(array<float>^ orientation);

	property float FocalDistance {
		float get();
		void set(float value);
	}

private:
	void rotateCamera(const SbVec3f & aroundaxis, const float delta);

	SoNode* _node;
};
}

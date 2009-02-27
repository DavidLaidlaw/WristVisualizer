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
	Camera(System::String^ scenegraph);
	!Camera();

	virtual SoNode* getNode() override { return _node; }

	void copySettingsFromCamera(Camera^ camera);

	void rotateCameraInX(const float movement);
	void rotateCameraInY(const float movement);

	property bool IsOrthographic {
		bool get();
	}

	array<float>^ getPosition();
	array<float>^ getOrientation();
	void setPosition(array<float>^ position);
	void setOrientation(array<float>^ orientation);

	property float FocalDistance {
		float get();
		void set(float value);
	}
	property float FarDistance {
		float get();
		void set(float value);
	}
	property float NearDistance {
		float get();
		void set(float value);
	}
	property float Height {
		float get();
		void set(float value);
	}

private:
	void rotateCamera(const SbVec3f & aroundaxis, const float delta);

	SoNode* _node;
};
}

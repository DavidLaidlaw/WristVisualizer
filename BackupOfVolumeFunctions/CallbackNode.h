#pragma once
#include "node.h"
#include <Inventor\nodes\SoCallback.h>//going to use callback for opengl 
#include "MasterCube.h"

namespace libCoin3D {
	public ref class CallbackNode : Node
	{
	public:
		CallbackNode();
		virtual ~CallbackNode();
		void setUpCallBack();
		virtual SoNode* getNode() override;
	
	private:	
		//callback node for opengl rendering
		SoCallback* volCallback;
		MasterCube* mCube;
	};
}


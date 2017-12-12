#pragma once
#include "node.h"
#include <Inventor\nodes\SoCallback.h>//going to use callback for opengl 
#include "MasterCube.h"

namespace libCoin3D {
	public ref class CallbackNode : Node
	{
	public:
		CallbackNode(int w ,int h, int l,int x, int y, int z, array<int>^ d,bool IsLeft);
		virtual ~CallbackNode();
		void setUpCallBack();
		virtual SoNode* getNode() override;
		void setDoDrawVolume(bool b);
		void setSliceNum(int num);
		void SetIsOpaque(bool b);
		void setOpacity(float o);

	private:	
		//callback node for opengl rendering
		SoCallback* volCallback;
		MasterCube* mCube;
		bool isLeft;
	};
}


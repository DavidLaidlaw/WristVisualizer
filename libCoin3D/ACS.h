#pragma once

#include "Node.h"
#include <Inventor/nodes/SoNode.h>
#include <Inventor/nodes/SoSeparator.h>

namespace libCoin3D {
	public ref class ACS : Node
	{
	public:
		ACS(int length);
		ACS();
		virtual SoNode* getNode() override;
	private: 
		virtual void makeACS(int length);
		SoSeparator* _acs;
	};
}

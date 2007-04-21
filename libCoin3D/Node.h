#pragma once

#include <Inventor/nodes/SoNode.h>

namespace libCoin3D {
public ref class Node abstract
{
public:
	explicit Node();
	//virtual ~Node()=0;
	//virtual void test(int t)=0;
	virtual SoNode* getNode()=0;
};
}

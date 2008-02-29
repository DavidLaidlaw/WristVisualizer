#pragma once
#include "Node.h"
#include <Inventor/nodes/SoSeparator.h>
#include <Inventor/nodes/SoTranslation.h>
#include <Inventor/nodes/SoFont.h>
#include <Inventor/nodes/SoText2.h>

namespace libCoin3D {
public ref class Label2D : Node
{
public:
	Label2D();
	~Label2D();
	void setText(System::String^ text);
	void setFontSize(int size);
	void setLocation(float x, float y);
	virtual SoNode* getNode() override { return _node; }
private:
	SoSeparator* _node;
	SoTranslation* _translation;
	SoFont* _font;
	SoText2* _text;
};
}

#pragma once
#include <Inventor/nodes/SoSeparator.h>
#include <Inventor/nodes/SoDrawStyle.h>
#include "Transform.h"



namespace libCoin3D {
public ref class Separator
{
public:
	Separator(void);
	void addChild(Separator^ child);
	void addFile(System::String^ filename);
	void addFile(System::String^ filename, bool canhide);
	SoSeparator* getSoSeparator(void);
	void addTransform(Transform^ transform);

	void hide();
	void show();

private:
	SoSeparator* _separator;
	SoDrawStyle* _style;
	
};
}

#pragma once
#include <Inventor/nodes/SoSeparator.h>
#include <Inventor/nodes/SoDrawStyle.h>

#include "TessellatedSurface.h"
#include "Transform.h"
#include "Node.h"


namespace libCoin3D {
public ref class Separator : Node
{
public:
	Separator(void);
	Separator(SoSeparator* node);
	void addChild(Separator^ child);
	void addFile(System::String^ filename);
	void addFile(System::String^ filename, bool canhide);
	void addNode(Node^ node);
	SoSeparator* getSoSeparator(void);
	void addTransform(Transform^ transform);
	void removeTransform();
	void insertNode(Node^ node, int position);
	void removeChild(Separator^ child);
	void removeChild(Node^ node);
	bool hasTransform() { return (_numTransforms>0); }
	virtual void reference();
	virtual void unref();
	virtual void unrefNoDelete();

	virtual SoNode* getNode() override;

	void hide();
	void show();

	TessellatedSurface^ findTeselatedSurface();

private:
	SoSeparator* _separator;
	SoDrawStyle* _style;
	int _numTransforms;

	
};
}

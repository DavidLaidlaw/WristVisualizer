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
	
	///////////////////////////////////
	void addChild(SoSeparator* child){
		_separator->addChild(child);
	}

	
	void addChild(Node^ child);
	void addChildToBeginning(Node^ child);
	void addChildAtIndex(Node^ child, int index);
	///////////////////////////////
	void addFile(System::String^ filename);
	void addFile(System::String^ filename, bool canhide);
	void addNode(Node^ node);
	SoSeparator* getSoSeparator(void);
	void addTransform(Transform^ transform);
	void removeTransform();
	void insertNode(Node^ node, int position);
	void removeChild(Separator^ child);
	void removeChild(Node^ node);
	bool hasTransform() { return (_transform != nullptr); }
	virtual void reference();
	virtual void unref();
	virtual void unrefNoDelete();

	virtual SoNode* getNode() override;

	void makeHideable();
	void SetVisibility(bool visible);
	//void hide();
	//void show();

	TessellatedSurface^ findTeselatedSurface();

private:
	SoSeparator* _separator;
	SoDrawStyle* _style;

	Transform^ _transform;
};
}

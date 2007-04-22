#pragma once

#include <Inventor/Win/viewers/SoWinExaminerViewer.h>
#include <Inventor/nodes/SoSeparator.h>

#include "Separator.h"

namespace libCoin3D {
public ref class ExaminerViewer
{
public:
	ExaminerViewer(int parrent);
	~ExaminerViewer();
	void setDecorator(bool decorate);
	void setSceneGraph(Separator^ root);
	bool saveToJPEG(System::String^ filename);
	bool saveToPNG(System::String^ filename);
	bool saveToGIF(System::String^ filename);
	bool saveToTIFF(System::String^ filename);
	bool saveToBMP(System::String^ filename);

	//background color stuff
	int getBackgroundColorR();
	int getBackgroundColorG();
	int getBackgroundColorB();
	void setBackgroundColor(float r, float g, float b);
private:
	bool saveToImage(System::String^ filename,char* ext);
	SoWinExaminerViewer* _viewer;
	SoSeparator* _root;
	bool _decorated;

};
}
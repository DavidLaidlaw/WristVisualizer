#pragma once

#include <Inventor/Win/viewers/SoWinExaminerViewer.h>
#include <Inventor/nodes/SoSeparator.h>
#include <Inventor/nodes/SoSelection.h>

#include <Inventor/nodes/SoEventCallback.h>

#include "Separator.h"

namespace libCoin3D {

	public delegate void RaypickEventHandler(float, float, float);

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
	float getBackgroundColorR();
	float getBackgroundColorG();
	float getBackgroundColorB();
	int getBackgroundColor();
	void setBackgroundColor(float r, float g, float b);
	void setBackgroundColor(int rgb);

	void setFeedbackVisibility(bool visible);

	//methods for raypicking, used for point selection
	void setRaypick();
	void resetRaypick();

	//events
	void fireClick(float x, float y, float z); //needs to be public, called from global static function
	event RaypickEventHandler^ OnRaypick;

	//static members, keeping track of all global ExaminerViewers
	static System::Collections::Hashtable^ ViewersHashtable = gcnew System::Collections::Hashtable();
	static ExaminerViewer^ getViewerByParentWidget(int HWND);

	//selectors, used for selections and selecting
	SoSelection* _selection;
	void enableSelection();
	void disableSelection();

private:
	bool saveToImage(System::String^ filename,char* ext);
	SoWinExaminerViewer* _viewer;
	SoSeparator* _root;
	bool _decorated;

	SoEventCallback* _ecb;
};
}
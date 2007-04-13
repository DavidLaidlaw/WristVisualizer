#pragma once

#include <Inventor/Win/viewers/SoWinExaminerViewer.h>

#include "Separator.h"

namespace libCoin3D {
public ref class ExaminerViewer
{
public:
	ExaminerViewer(int parrent);
	~ExaminerViewer();
	void setDecorator(bool decorate);
	void setSceneGraph(Separator^ root);
private:
	SoWinExaminerViewer* _viewer;
	bool _decorated;

};
}
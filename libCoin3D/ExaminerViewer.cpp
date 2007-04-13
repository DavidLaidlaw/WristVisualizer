#include "StdAfx.h"
#include "ExaminerViewer.h"

#include <Inventor/nodes/SoSeparator.h> // remove me later
#include <Inventor/nodes/SoCone.h>      // remove me later
#include <Inventor/nodes/SoSphere.h>      // remove me later
#include <Inventor/nodes/SoPerspectiveCamera.h>
#include <Inventor/nodes/SoOrthographicCamera.h>

libCoin3D::ExaminerViewer::ExaminerViewer(int parrent)
{
	_viewer = NULL;
	_decorated = TRUE;
	_viewer = new SoWinExaminerViewer((HWND)parrent);

	SoSeparator *root = new SoSeparator; // remove me later
    //root->addChild(new SoCone);          // remove me later
    _viewer->setSceneGraph(root);         // remove me later
	_viewer->setCameraType(SoOrthographicCamera::getClassTypeId());
}

libCoin3D::ExaminerViewer::~ExaminerViewer()
{
	if (_viewer != NULL)
		delete _viewer;
}

void libCoin3D::ExaminerViewer::setDecorator(bool decorate)
{
	if (_viewer != NULL && _decorated!=decorate) {
		_decorated = decorate;
		_viewer->setDecoration(decorate);
	}
	

}

void libCoin3D::ExaminerViewer::setSceneGraph(Separator^ root)
{
	if (_viewer != NULL)
		_viewer->setSceneGraph(root->getSoSeparator());
}

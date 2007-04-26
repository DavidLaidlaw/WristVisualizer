#include "StdAfx.h"
#include "ExaminerViewer.h"

 // remove me later
#include <Inventor/nodes/SoCone.h>      // remove me later
#include <Inventor/nodes/SoSphere.h>      // remove me later
#include <Inventor/nodes/SoPerspectiveCamera.h>
#include <Inventor/nodes/SoOrthographicCamera.h>


//for output
#include <Inventor/SoOffscreenRenderer.h>
#include <Inventor/actions/SoGLRenderAction.h>
#include <Inventor/actions/SoWriteAction.h>

libCoin3D::ExaminerViewer::ExaminerViewer(int parrent)
{
	_viewer = NULL;
	_decorated = TRUE;
	_viewer = new SoWinExaminerViewer((HWND)parrent);

	_root = new SoSeparator; // remove me later
    //root->addChild(new SoCone);          // remove me later
    _viewer->setSceneGraph(_root);         // remove me later
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
	_root = root->getSoSeparator();
	if (_viewer != NULL)
		_viewer->setSceneGraph(root->getSoSeparator());
}


bool libCoin3D::ExaminerViewer::saveToJPEG(System::String ^filename)
{
	return saveToImage(filename,"jpg");
}
bool libCoin3D::ExaminerViewer::saveToPNG(System::String ^filename)
{
	return saveToImage(filename,"png");
}
bool libCoin3D::ExaminerViewer::saveToGIF(System::String ^filename)
{
	return saveToImage(filename,"gif");
}
bool libCoin3D::ExaminerViewer::saveToTIFF(System::String ^filename)
{
	//TODO: Fix error with saveing screenshots to other formats - include simage.lib?
	//test Code....don't know why I can't load these other types...
	/*
	SbPList extlist;
    SbString fullname, description;
	SoOffscreenRenderer * r = new SoOffscreenRenderer(*(new SbViewportRegion));
	int num = r->getNumWriteFiletypes();
	for (int i=0; i<num; i++) {
    r->getWriteFiletypeInfo(i, extlist, fullname, description);
	fullname += ": ";
	fullname += description;
	System::Console::Write(fullname.getString());
    //(void)fprintf(stdout, "%s: %s (extension%s: ",
      //                fullname.getString(), description.getString(),
        //              extlist.getLength() > 1 ? "s" : "");
        for (int j=0; j < extlist.getLength(); j++) {
			System::Console::Write((const char*) extlist[j]);
          //(void)fprintf(stdout, "%s%s", j>0 ? ", " : "", (const char*) extlist[j]);
        }
		System::Console::WriteLine("");
   //(void)fprintf(stdout, ")\n");
	}
	delete r;
	*/
	return saveToImage(filename,"tiff");
}
bool libCoin3D::ExaminerViewer::saveToBMP(System::String ^filename)
{
	return saveToImage(filename,"bmp");
}

bool libCoin3D::ExaminerViewer::saveToImage(System::String ^filename, char *ext) 
{
	char* fname = (char*)System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(filename).ToPointer();

	//SoOutput *postfile = new SoOutput;
	//postfile->openFile(fname);
  
	const SbViewportRegion &vp  = _viewer->getViewportRegion();
	const SbVec2s &imagePixSize = vp.getViewportSizePixels();
	SbVec2f imageInches;
	float pixPerInch;
	float quality = 1;
	int screenDPI = 400;
  
	//pixPerInch = SoOffscreenRenderer::getScreenPixelsPerInch();
	pixPerInch = 300;
	imageInches.setValue((float)imagePixSize[0] / pixPerInch,
		       (float)imagePixSize[1] / pixPerInch);
  
    // The resolution to render the scene for the printer
	// is equal to the size of the image in inches times
	// the printer DPI;
	SbVec2s postScriptRes;
    postScriptRes.setValue((short)(imageInches[0]*screenDPI),
                          (short)(imageInches[1]*screenDPI));

    // Create a viewport to render the scene into.
    SbViewportRegion myViewport;
    myViewport.setWindowSize(postScriptRes);
    myViewport.setPixelsPerInch((float)screenDPI);

	// Render the scene
	SoGLRenderAction *newRA = new SoGLRenderAction(myViewport);
	newRA->setTransparencyType(SoGLRenderAction::BLEND);    
	SoOffscreenRenderer *myRenderer = new SoOffscreenRenderer(newRA);
	myRenderer->setBackgroundColor(_viewer->getBackgroundColor());


    if (!myRenderer->render(_viewer->getSceneManager()->getSceneGraph())) {  //render root?
		delete myRenderer;
		System::Console::WriteLine("Couldn't capture root of tree");
		return false;
    }


//    if (!myRenderer->render(root)) {
//	delete myRenderer;
//	return FALSE;
//  }

    // Generate PostScript and write it to the given file
    //myRenderer->writeToRGB(postfile->getFilePointer());
	bool result = myRenderer->writeToFile(fname,ext);
	//myRenderer->writeToJPEG(postfile->getFilePointer(), quality);

    delete myRenderer;
	//postfile->closeFile();

	return result;

}
float libCoin3D::ExaminerViewer::getBackgroundColorR()
{
	return _viewer->getBackgroundColor()[0];
}
float libCoin3D::ExaminerViewer::getBackgroundColorG()
{
	return _viewer->getBackgroundColor()[1];
}
float libCoin3D::ExaminerViewer::getBackgroundColorB()
{
	return _viewer->getBackgroundColor()[2];
}

int libCoin3D::ExaminerViewer::getBackgroundColor()
{
	return _viewer->getBackgroundColor().getPackedValue();
}

void libCoin3D::ExaminerViewer::setBackgroundColor(float r, float g, float b)
{
	_viewer->setBackgroundColor(SbColor(r,g,b));
}

void libCoin3D::ExaminerViewer::setBackgroundColor(int rgb) 
{
	SbColor c;
	float junk=0;
	c.setPackedValue(rgb,junk);
	_viewer->setBackgroundColor(c);
}

void libCoin3D::ExaminerViewer::setFeedbackVisibility(bool visible)
{
	//only change, if its different
	if (_viewer->isFeedbackVisible() != visible)
		_viewer->setFeedbackVisibility(visible);

}
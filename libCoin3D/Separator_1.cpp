#include "StdAfx.h"

#include <Inventor/actions/SoSearchAction.h>
#include <Inventor/nodes/SoCoordinate3.h>
#include <Inventor/nodes/SoIndexedFaceSet.h>

#include "Separator.h"

libCoin3D::Separator::Separator(void)
{
	_separator = new SoSeparator();
	_style = NULL;
	_transform = nullptr;
}

libCoin3D::Separator::Separator(SoSeparator* node)
{
	_separator = node;
	_style = NULL;
}


void libCoin3D::Separator::addChild(Separator^ child)
{
	_separator->addChild(child->getSoSeparator());
}


void libCoin3D::Separator::addFile(System::String^ filename)
{
	addFile(filename,false);
}
void libCoin3D::Separator::addFile(System::String^ filename, bool canhide)
{
	System::String^ shortFilename = System::IO::Path::GetFileName(filename);
	//make shortFilename safe for coin3d
	shortFilename = shortFilename->Replace('.', '_');
	shortFilename = shortFilename->Replace(' ', '_');
	if (shortFilename->IndexOfAny(((System::String^)"0123456789")->ToCharArray(),0,1)==0) //test if it start with a number
		shortFilename = "_" + shortFilename;
	char* test = (char*)System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(filename).ToPointer();
	char* charShortFilename = (char*)System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(shortFilename).ToPointer();

	_style = new SoDrawStyle;

	SoInput in;
	if ( in.openFile(test) ) {
		SoSeparator *bone = SoDB::readAll( &in );
		if (bone==NULL) //small error checking
			throw gcnew System::ArgumentException("Error parsing IV file: "+filename);
		_separator->addChild(_style);
		_separator->addChild(bone);
		bone->setName(charShortFilename);
	}

	System::Runtime::InteropServices::Marshal::FreeHGlobal((System::IntPtr)test);
	System::Runtime::InteropServices::Marshal::FreeHGlobal((System::IntPtr)charShortFilename);
}

void libCoin3D::Separator::addNode(Node^ node)
{
	if (node != nullptr && node->getNode() != NULL)
		_separator->addChild(node->getNode());
}

void libCoin3D::Separator::insertNode(Node^ node, int position)
{
	_separator->insertChild(node->getNode(), position);
}

void libCoin3D::Separator::removeChild(Separator^ child)
{
	_separator->removeChild(child->getSoSeparator());
}
void libCoin3D::Separator::removeChild(Node^ child)
{
	_separator->removeChild(child->getNode());
}

void libCoin3D::Separator::makeHideable()
{
	if (_style != NULL) return;
	_style = new SoDrawStyle;
	_separator->addChild(_style);
}

//void libCoin3D::Separator::hide()
//{
//	if (_style != NULL) 
//		_style->style = SoDrawStyle::INVISIBLE;
//}

//void libCoin3D::Separator::show()
//{
//	if (_style != NULL) 
//		_style->style = SoDrawStyle::FILLED;
//}

void libCoin3D::Separator::SetVisibility(bool visible)
{
	if (_style==NULL) return; //no style, do nothing :)

	if (visible)
		_style->style = SoDrawStyle::FILLED;
	else
		_style->style = SoDrawStyle::INVISIBLE;
}

SoSeparator* libCoin3D::Separator::getSoSeparator(void)
{
	return _separator;
}

void libCoin3D::Separator::addTransform(libCoin3D::Transform ^transform)
{
	if (_transform != nullptr)
		throw gcnew System::ArgumentException("Cannot add transform to separator, one transform already present");

	_separator->insertChild(transform->getSoTransform(),0);
	_transform = transform;
}

void libCoin3D::Separator::removeTransform()
{
	if (_transform != nullptr)
		_separator->removeChild(_transform->getNode());
	_transform = nullptr;
}

SoNode* libCoin3D::Separator::getNode()
{
	return _separator;
}

libCoin3D::TessellatedSurface^ libCoin3D::Separator::findTeselatedSurface()
{
	SoSearchAction sa;
	sa.setType(SoCoordinate3::getClassTypeId(),false);
	//by default only gets the first path
	//sa.setInterest(SoSearchAction::ALL);
	sa.apply(_separator);
	SoPath* myPath = sa.getPath();
	if (myPath==NULL)
		return nullptr; //no teselated surfaces found

	if (myPath->getLength() < 2) {
		fprintf(stderr,"Problem, no parrent!\n"); //TODO: How did we reach here, can we even?
		return nullptr;
	}

	SoCoordinate3* coords = (SoCoordinate3*)myPath->getTail();
	SoGroup* parent = (SoGroup*)myPath->getNodeFromTail(1);
	//now I need to go find the faceSet (connections)
	SoIndexedFaceSet* conn = NULL;
	int coordIndex = parent->findChild(coords);
	assert(coordIndex >= 0); //it must be greater, we know its the parent
	//now to start at index+1, its after the node we know
	for (int i=coordIndex + 1; i < parent->getNumChildren(); i++) {
		//look for an IndexedFaceSet && NOT another Coordinate3 node
		if (parent->getChild(i)->isOfType(SoIndexedFaceSet::getClassTypeId())) {
			conn = (SoIndexedFaceSet*)parent->getChild(i);
			break;
		}

		if (parent->getChild(i)->isOfType(SoCoordinate3::getClassTypeId()))
			return nullptr;
	}

	//at this point we should have both the Index and Coordinates
	TessellatedSurface^ ts = gcnew TessellatedSurface(coords,conn);
	return ts;
}

void libCoin3D::Separator::reference()
{
	_separator->ref();
}

void libCoin3D::Separator::unref()
{
	_separator->unref();
}

void libCoin3D::Separator::unrefNoDelete()
{
	_separator->unrefNoDelete();
}
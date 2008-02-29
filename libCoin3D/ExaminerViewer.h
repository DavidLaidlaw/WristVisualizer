#pragma once

#include <Inventor/Win/viewers/SoWinExaminerViewer.h>
#include <Inventor/nodes/SoSeparator.h>
#include <Inventor/nodes/SoSelection.h>

#include <Inventor/nodes/SoEventCallback.h>

#include "Separator.h"
#include "ScenegraphNode.h"
#include "Material.h"

namespace libCoin3D {

	public delegate void RaypickEventHandler(float, float, float);
	public delegate void ObjectSelectedHandler();
	public delegate void ObjectDeselectedHandler();
	public delegate void NewSceneGraphLoadedHandler();

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
	void saveSceneGraph(System::String^ filename);

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
	event NewSceneGraphLoadedHandler^ OnNewSceneGraphLoaded;

	//static members, keeping track of all global ExaminerViewers
	static System::Collections::Hashtable^ ViewersHashtable = gcnew System::Collections::Hashtable();
	static ExaminerViewer^ getViewerByParentWidget(int HWND);

	//selectors, used for selections and selecting
	SoSelection* _selection;
	event ObjectSelectedHandler^ OnObjectSelected;
	event ObjectDeselectedHandler^ OnObjectDeselected;
	void fireChangeObjectSelection(bool selected);
	Material^ getSelectedMaterial();
	Material^ createMaterialForSelected();
	void removeMaterialFromScene(Material^ material);
	void setSelection(ScenegraphNode^ node);

	//get and set transparency rendering type
	enum class TransparencyTypes {
		SCREEN_DOOR,
		ADD, DELAYED_ADD, SORTED_OBJECT_ADD,
		BLEND, DELAYED_BLEND, SORTED_OBJECT_BLEND,
		// The remaining are Coin extensions to the common Inventor API
		SORTED_OBJECT_SORTED_TRIANGLE_ADD,
		SORTED_OBJECT_SORTED_TRIANGLE_BLEND
	};
	TransparencyTypes getTransparencyType();
	void setTransparencyType(TransparencyTypes type);

	void setDrawStyle();
	void viewAll();

private:
	bool saveToImage(System::String^ filename,char* ext);
	SoWinExaminerViewer* _viewer;
	SoSeparator* _root;
	bool _decorated;

	SoEventCallback* _ecb;

	//private calls for finding material nodes. Used for material editing
	SoNode* getSelectedNode();
	SoGroup* getParentOfNode(SoNode* child);
	SoGroup* getParentOfSelectedNode();
	SoMaterial* getMaterialForSelectedNode();
	SoMaterial* createMaterialForSelectedNode();
	SoMaterial* getMaterialPropertiesAtNode(SoNode* node);
};
}
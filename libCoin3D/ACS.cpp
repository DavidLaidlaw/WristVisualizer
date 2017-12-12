#include "StdAfx.h"
#include "ACS.h"

#include <Inventor/nodes/SoSeparator.h>
#include <Inventor/nodes/SoSphere.h>
#include <Inventor/nodes/SoCylinder.h>
#include <Inventor/nodes/SoCone.h>
#include <Inventor/nodes/SoMaterial.h>
#include <Inventor/nodes/SoTransform.h>

libCoin3D::ACS::ACS()
{
	makeACS(20);  //20 is the default length
}

libCoin3D::ACS::ACS(int length)
{
	makeACS(length);
}

void libCoin3D::ACS::makeACS(int length)
{
	_acs = new SoSeparator();

	//first make X cylinder
	SoSeparator* x = new SoSeparator();
	SoMaterial* x_material = new SoMaterial();
	x_material->diffuseColor.setValue(1,0,0);
	x->addChild(x_material);
	SoTransform* x_transform = new SoTransform();
	x_transform->translation.setValue((float)length/2,0,0);
	x_transform->rotation.setValue(SbVec3f(0, 0, -1),1.5708f);
	x->addChild(x_transform);
	SoCylinder* x_cylinder = new SoCylinder();
	x_cylinder->radius.setValue(0.5);
	x_cylinder->height.setValue((float)length);
	x->addChild(x_cylinder);
	_acs->addChild(x);

	
	//make x cone
	SoSeparator* x2 = new SoSeparator();
	SoMaterial* x2_material = new SoMaterial();
	x2_material->diffuseColor.setValue(1,0,0);
	x2->addChild(x2_material);
	SoTransform* x2_transform = new SoTransform();
	x2_transform->translation.setValue((float)length,0,0);
	x2_transform->rotation.setValue(SbVec3f(0, 0, -1),1.5708f);
	x2->addChild(x2_transform);
	SoCone* x_cone = new SoCone();
	x_cone->bottomRadius.setValue(1);
	x_cone->height.setValue(3);
	x2->addChild(x_cone);
	_acs->addChild(x2);
	

	//make Y cylinder
	SoSeparator* y = new SoSeparator();
	SoMaterial* y_material = new SoMaterial();
	y_material->diffuseColor.setValue(0,1,0);
	y->addChild(y_material);
	SoTransform* y_transform = new SoTransform();
	y_transform->translation.setValue(0,(float)length/2,0);
	//y_transform->rotation.setValue(0,0,0,0);
	y->addChild(y_transform);
	SoCylinder* y_cylinder = new SoCylinder();
	y_cylinder->radius.setValue(0.5);
	y_cylinder->height.setValue((float)length);
	y->addChild(y_cylinder);
	_acs->addChild(y);

	//make y cone
	SoSeparator* y2 = new SoSeparator();
	SoMaterial* y2_material = new SoMaterial();
	y2_material->diffuseColor.setValue(0,1,0);
	y2->addChild(y2_material);
	SoTransform* y2_transform = new SoTransform();
	y2_transform->translation.setValue(0,(float)length,0);
	//y2_transform->rotation.setValue(0,0,0,0);
	y2->addChild(y2_transform);
	SoCone* y_cone = new SoCone();
	y_cone->bottomRadius.setValue(1);
	y_cone->height.setValue(3);
	y2->addChild(y_cone);
	_acs->addChild(y2);

	//make Z cylinder
	SoSeparator* z = new SoSeparator();
	SoMaterial* z_material = new SoMaterial();
	z_material->diffuseColor.setValue(0,0,1);
	z->addChild(z_material);
	SoTransform* z_transform = new SoTransform();
	z_transform->translation.setValue(0,0,(float)length/2);
	z_transform->rotation.setValue(SbVec3f(1, 0, 0),1.5708f);
	z->addChild(z_transform);
	SoCylinder* z_cylinder = new SoCylinder();
	z_cylinder->radius.setValue(0.5);
	z_cylinder->height.setValue((float)length);
	z->addChild(z_cylinder);
	_acs->addChild(z);

	
	//make Z cone
	SoSeparator* z2 = new SoSeparator();
	SoMaterial* z2_material = new SoMaterial();
	z2_material->diffuseColor.setValue(0,0,1);
	z2->addChild(z2_material);
	SoTransform* z2_transform = new SoTransform();
	z2_transform->translation.setValue(0,0,(float)length);
	z2_transform->rotation.setValue(SbVec3f(1, 0, 0),1.5708f);
	z2->addChild(z2_transform);
	SoCone* z_cone = new SoCone();
	z_cone->bottomRadius.setValue(1);
	z_cone->height.setValue(3);
	z2->addChild(z_cone);
	_acs->addChild(z2);
}

SoNode* libCoin3D::ACS::getNode()
{
	return _acs;
}

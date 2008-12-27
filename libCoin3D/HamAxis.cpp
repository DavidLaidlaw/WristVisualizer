#include "StdAfx.h"
#include "HamAxis.h"
#include <Inventor/nodes/SoCylinder.h>
#include <Inventor/nodes/SoCone.h>
#include <Inventor/nodes/SoTransform.h>

libCoin3D::HamAxis::HamAxis(float Nx, float Ny, float Nz, float Qx, float Qy, float Qz)
{
	makeHam(Nx, Ny, Nz, Qx, Qy, Qz, NULL);
}

libCoin3D::HamAxis::HamAxis(double Nx, double Ny, double Nz, double Qx, double Qy, double Qz)
{
	makeHam((float)Nx, (float)Ny, (float)Nz, (float)Qx, (float)Qy, (float)Qz, NULL);
}

libCoin3D::HamAxis::HamAxis(Material^ m, double Nx, double Ny, double Nz, double Qx, double Qy, double Qz)
{
	makeHam((float)Nx, (float)Ny, (float)Nz, (float)Qx, (float)Qy, (float)Qz, (SoMaterial*)m->getNode());
}

void libCoin3D::HamAxis::makeHam(float Nx, float Ny, float Nz, float Qx, float Qy, float Qz, SoMaterial* m)
{
	float N[3] = {Nx, Ny, Nz};
	float Q[3] = {Qx, Qy, Qz};

	//check for "invalid HAM"
	if (Qx==0 && Qy==0 && Qz==0) {
		_node = NULL;
		return;
	}

	float fromVec[3] = {0, 1, 0};
	const float hamAxesHeight = 60;
	const float hamAxesRadius = 0.5;

    // make axis separator
    SoSeparator *hamAxis = new SoSeparator;
	hamAxis->ref();

	if (m != NULL)
		hamAxis->addChild(m);	// Color appropriately

	SoTransform *axes_transform = new SoTransform;
	_cylinder = new SoCylinder;
	_cone = new SoCone;
	_coneTransform = new SoTransform;

    hamAxis->addChild(axes_transform);		// move so Q is center
	hamAxis->addChild(_cylinder);				// add cylinder of arbitrary length    
	hamAxis->addChild(_coneTransform);
	hamAxis->addChild(_cone);

    SbRotation axesRot;

    
    axesRot.setValue(fromVec, N);				// rotation from normal cylinder to N
    axes_transform->rotation.setValue(axesRot);
    
    axes_transform->translation.setValue(Q);	// translation so cylinder centered on point Q

	// set the color for the HAM axis
 //   SoMaterial *axesMaterial = new SoMaterial;
 //   SbVec3f Col; 
 //   
	//int bint = boneint;  
 //   Col.setValue(hamColors[bint][0], hamColors[bint][1], hamColors[bint][2]);
 //   axesMaterial->diffuseColor.setValue(Col);

	_cone->bottomRadius = hamAxesRadius/AXIS_CONE_RADIUS_RATIO;
	_cone->height = hamAxesHeight/AXIS_CONE_LENGTH_RATIO; 
	
	_coneTransform->translation.setValue(0,hamAxesHeight/2 + (hamAxesHeight/AXIS_CONE_LENGTH_RATIO)/2,0);
    _cylinder->radius = hamAxesRadius;
    _cylinder->height = hamAxesHeight;

	hamAxis->unrefNoDelete();
    _node = hamAxis; //save it as the local node :)
}

void libCoin3D::HamAxis::SetHamDimensions(float length, float radius)
{
	SetHamLength(length);
	SetHamRadius(radius);
}

void libCoin3D::HamAxis::SetHamLength(float length)
{
	if (_node == NULL) return;
	_cylinder->height = length;
	_cone->height = length/AXIS_CONE_LENGTH_RATIO;
	_coneTransform->translation.setValue(0,length/2 + (length/AXIS_CONE_LENGTH_RATIO)/2,0);
}

void libCoin3D::HamAxis::SetHamRadius(float radius)
{
	if (_node == NULL) return;
	_cylinder->radius = radius;
	_cone->bottomRadius = radius/AXIS_CONE_RADIUS_RATIO;
}


void libCoin3D::HamAxis::setColor(float r, float g, float b)
{
	if (_node==NULL) return;
	SoMaterial* axesMaterial = new SoMaterial();
	SbVec3f Col; 
	Col.setValue(r,g,b);
	axesMaterial->diffuseColor.setValue(Col);
	SoSeparator* realNode = (SoSeparator*)_node;
	realNode->insertChild(axesMaterial,0);
}
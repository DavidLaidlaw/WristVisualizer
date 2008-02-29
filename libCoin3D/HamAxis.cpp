#include "StdAfx.h"
#include "HamAxis.h"
#include <Inventor/nodes/SoCylinder.h>
#include <Inventor/nodes/SoCone.h>
#include <Inventor/nodes/SoTransform.h>

libCoin3D::HamAxis::HamAxis(float Nx, float Ny, float Nz, float Qx, float Qy, float Qz)
{
	makeHam(Nx, Ny, Nz, Qx, Qy, Qz);
}

libCoin3D::HamAxis::HamAxis(double Nx, double Ny, double Nz, double Qx, double Qy, double Qz)
{
	makeHam((float)Nx, (float)Ny, (float)Nz, (float)Qx, (float)Qy, (float)Qz);
}

void libCoin3D::HamAxis::makeHam(float Nx, float Ny, float Nz, float Qx, float Qy, float Qz)
{
	float N[3] = {Nx, Ny, Nz};
	float Q[3] = {Qx, Qy, Qz};

	//check for "invalid HAM"
	if (Qx==0 && Qy==0 && Qz==0) {
		_node = NULL;
		return;
	}

	float fromVec[3] = {0, 1, 0};
	float hamAxesHeight = 60;
    SbRotation axesRot;


    SoTransform *axes_transform = new SoTransform;
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

    SoCylinder *segment = new SoCylinder;
	SoCone *myCone = new SoCone;
	myCone->bottomRadius = 1;
	myCone->height = 4; 
	SoTransform *coneTransform = new SoTransform;
	coneTransform->translation.setValue(0,hamAxesHeight/2,0);
    segment->radius = 0.5;
    segment->height = hamAxesHeight;

    // make axis separator
    SoSeparator *hamAxis = new SoSeparator;
    //hamAxis->addChild(axesMaterial);		// Color appropriately
    hamAxis->addChild(axes_transform);		// move so Q is center
	hamAxis->addChild(segment);				// add cylinder of arbitrary length    
	hamAxis->addChild(coneTransform);
	hamAxis->addChild(myCone);
    
    _node = hamAxis; //save it as the local node :)
}
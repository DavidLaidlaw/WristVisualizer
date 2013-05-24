#include "StdAfx.h"
#include "Scale.h"


libCoin3D::Scale::Scale(void)
{
	_scale=new SoScale();
}


void libCoin3D::Scale::reference()
{
	_scale->ref();
}

void libCoin3D::Scale::setScaleFactor(float x, float y, float z){
	_scale->scaleFactor.setValue(x,y,z);
	//myScale->scaleFactor.setValue(16,16,16);
}

SoNode* libCoin3D::Scale::getNode()
{
	return _scale;
}

}
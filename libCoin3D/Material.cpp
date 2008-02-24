#include "StdAfx.h"
#include "Material.h"

libCoin3D::Material::Material()
{
	_material = new SoMaterial();
}

void libCoin3D::Material::setColor(float r, float g, float b)
{
	_material->diffuseColor.setValue(r,g,b);
}

void libCoin3D::Material::setTransparency(float transparency)
{
	_material->transparency.setValue(transparency);
}

SoNode* libCoin3D::Material::getNode()
{
	return _material;
}

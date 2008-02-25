#include "StdAfx.h"
#include "Material.h"

libCoin3D::Material::Material()
{
	_material = new SoMaterial();
}

libCoin3D::Material::Material(SoMaterial* material)
{
	_material = material;
	if (_material==NULL)
		_material = new SoMaterial();
}

void libCoin3D::Material::setColor(float r, float g, float b)
{
	_material->diffuseColor.setValue(r,g,b);
}

void libCoin3D::Material::setTransparency(float transparency)
{
	_material->transparency.setValue(transparency);
	_material->transparency.setIgnored(FALSE);
}

float libCoin3D::Material::getTransparency()
{
	return _material->transparency.getValues(0)[0];
}

int libCoin3D::Material::getColor()
{
	return _material->diffuseColor.getValues(0)->getPackedValue();
}

SoNode* libCoin3D::Material::getNode()
{
	return _material;
}

#include "StdAfx.h"
#include "Material.h"

libCoin3D::Material::Material()
{
	_material = new SoMaterial();
	_material->ref();
}

libCoin3D::Material::~Material()
{
	_material->unref();
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
	_material->diffuseColor.setIgnored(FALSE); //TODO: make more smart :-P
}

void libCoin3D::Material::setColor(System::Drawing::Color color)
{
	setColor(color.R/255.0f, color.G/255.0f, color.B/255.0f);
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

int libCoin3D::Material::getPackedColor()
{
	return _material->diffuseColor.getValues(0)->getPackedValue();
}

System::Drawing::Color libCoin3D::Material::getColor()
{
	int packedColor = getPackedColor();
	packedColor = (packedColor >> 8);
	return System::Drawing::Color::FromArgb(packedColor);
}

System::Drawing::Color libCoin3D::Material::GetDefaultColor()
{
	return System::Drawing::Color::FromArgb(255,204,204,204);
}

void libCoin3D::Material::setOverride(bool forceOverride)
{
	_material->setOverride(forceOverride);
}

SoNode* libCoin3D::Material::getNode()
{
	return _material;
}

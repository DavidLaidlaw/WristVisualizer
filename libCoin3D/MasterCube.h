#pragma once

#include <vector>
#include "ProxyPiece.h"
#include "CubeGeometry.h"
#include "Evaluator.h"
#include "ShaderHandler.h"
#include "Vector.h"
#include "TextureHandler.h"

using namespace std;

class MasterCube
{
public:
	MasterCube(int w,int h, int l,int x,int y,int z, int* d, bool IsLeft);
	~MasterCube(void);

	void renderCube();
	void drawVolumes();
	void enableTexturing();
	void disableTexturing();
	void initializeGlew();
	void createAndDrawProxyPieces();
	Vector3 getViewingAngle();//just return (0,0,-1) for now?
	void setProxNum(int num);
	void setDoDrawVolume(bool b);
	void setSliceNum(int num);
	void setIsOpaque(bool b);
	void setOpacity(float o);

private:

	CubeGeometry cube;
	vector<ProxyPiece> proxyPieces;
	int numProxies;
	int origNumSlices;

	Evaluator eval;

	float max;
	float curr;

	int boneBindingLoc;

	float opacityValue;
	float threshValue;
	float startThresh;
	int isOpaque;
	int calcNormsOnFly;

	bool drawOutlines;
	bool drawTexture;
	//no interacting mesh;
	int colorByNormals;
	bool isLeft;

	ShaderHandler shader;
	//no draw methods
	Vector3 textureSize;
	bool glewIsInit;

	Vector3 length;
	Vector3 voxSize;
	int* data;//the texture data
	TextureHandler texHandler;

	int actNumProx;

	bool doDrawVolume;
};


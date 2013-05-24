#pragma once

#include <vector>
#include "ProxyPiece.h"
#include "CubeGeometry.h"
#include "Evaluator.h"
#include "ShaderHandler.h"
#include "Vector.h"

using namespace std;

class MasterCube
{
public:
	MasterCube(void);
	~MasterCube(void);

	void renderCube();
	void drawVolumes();
	void enableTexturing();
	void disableTexturing();
	void initializeGlew();
	void createAndDrawProxyPieces();
	Vector3 getViewingAngle();//just return (0,0,-1) for now?

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

	ShaderHandler shader;
	//no draw methods
	Vector3 textureSize;
	bool glewIsInit;

	Vector3 length;
};


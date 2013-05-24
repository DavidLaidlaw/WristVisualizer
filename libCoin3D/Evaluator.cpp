#include "StdAfx.h"
#include "Evaluator.h"


Evaluator::Evaluator(void)
{
}


Evaluator::~Evaluator(void)
{
}


bool Evaluator::doesPlaneIntersectLineSegment(Vector3 start, Vector3 end, Vector3 normal, Vector3 point){
	Vector3 lineStart(start.x-point.x,start.y-point.y,start.z-point.z);
	Vector3 lineEnd(end.x-point.x,end.y-point.y,end.z-point.z);
	Vector3 ray(end.x-start.x,end.y-start.y,end.z-start.z);
	if (normal.dot(ray)==0){
		return false;
	}
	float a=normal.dot(lineStart);
	float b=normal.dot(lineEnd);

	if((a<0 && b>0)||(a>0 && b<0)){
		return true;
	}
	else{
		return false;
	}
}


Vector3 Evaluator::intersectPlaneAndLineSegment(Vector3 start, Vector3 end, Vector3 normal, Vector3 point){
	float d=normal.dot(point);
	Vector3 ray(end.x-start.x,end.y-start.y,end.z-start.z);

	if (!doesPlaneIntersectLineSegment(start,end,normal,point)){
		return Vector3(NULL,NULL,NULL);
	}
	else{
		float t=(d-normal.dot(start))/normal.dot(ray);

		//first normalize the ray
		//scale the ray by t
		Vector3 newRay(ray.x*t,ray.y*t,ray.z*t);
		Vector3 contact(start.x+newRay.x,start.y+newRay.y,start.z+newRay.z);

		if (t >= 0.0f && t <= 1.0f) {
			return contact; // line intersects plane
		}
		else {
			return Vector3(NULL,NULL,NULL);
		}
	}
}


//returns the points that bound a proxy geometry
vector<Vector3> Evaluator::findAllIntersections(CubeGeometry cube, Plane p){
	//System.out.println("\n new plane");
	Segment* segs=cube.getSegments();
	vector<Vector3> actualPoints;
	int inter=0;
	int segSize=12;
	for(int i=0;i<segSize;i++){
		Segment s=segs[i];
		if(this->doesPlaneIntersectLineSegment(s.getStart(), s.getEnd(), p.getNormal(), p.getPoint())){
			//System.out.println("intersected segment i: "+i);
			inter=inter+1;
			Vector3 contact=this->intersectPlaneAndLineSegment(s.getStart(), s.getEnd(), p.getNormal(), p.getPoint());
			actualPoints.push_back(contact);//add the bounding point to the vector
		}
		/*else{
			printf("DID NOT INTERSECT segment %i: \n",+i);
		}*/
	}

	return actualPoints;
}



ProxyPiece Evaluator::createSingleProxyPiece(CubeGeometry cube, Plane p){
	vector<Vector3> corners=findAllIntersections(cube,p);
	return ProxyPiece(p, corners,cube);
}


Vector3 Evaluator::findNearestCornerOnLine(CubeGeometry cube, Vector3 dir){
	//project each corner onto the line and find the lowest value one
	Vector3* corners=cube.getCorners();
	//normalize dir
	dir.normalize();//normalize dir

	Vector3 minCorner=corners[0];
	float min=FLT_MAX;

	int cornerLengh=8;
	for(int i=0;i<cornerLengh;i++){
		Vector3 c=corners[i];
		float pc=c.dot(dir);//project corner onto dir
		if(pc<min){
			min=pc;
			minCorner=c;
		}
	}
	return minCorner;
}


Vector3 Evaluator::findFarthestCornerOnLine(CubeGeometry cube, Vector3 dir){
	//project each corner onto the line and find the lowest value one
	Vector3* corners=cube.getCorners();
	//normalize dir
	dir.normalize();
	Vector3 maxCorner=corners[0];
	float max=FLT_MIN;

	int cornerLengh=8;
	for(int i=0;i<cornerLengh;i++){
		Vector3 c=corners[i];
		float pc=c.dot(dir);//project corner onto dir
		if(pc>max){
			max=pc;
			maxCorner=c;
		}
	}
	return maxCorner;
}


vector<ProxyPiece> Evaluator::createProxyPieces(CubeGeometry cube, Vector3 viewDir, Vector3 startLoc, Vector3 endLoc, int numPieces){
	vector<ProxyPiece> allProxyPieces;
	//depending on view direction
	//spacing is an adjustment so it only moves a certain fraction per index

	Vector3 normDir=(viewDir);
	normDir.normalize();

	startLoc=this->findNearestCornerOnLine(cube, viewDir);
	endLoc=this->findFarthestCornerOnLine(cube, viewDir);
	//now find appropriate spacing
	
	float dist=(endLoc-startLoc).length();
	float spacing=dist/numPieces;

	//calculate all points at which there should be a plane
	for(int i=0;i<numPieces;i++){
		//Vector3 planePoint=new Vector3(startLoc.x+i*spacing,startLoc.y+i*spacing,startLoc.z+i*spacing);
		Vector3 planePoint(startLoc.x+i*spacing*normDir.x,startLoc.y+i*spacing*normDir.y,startLoc.z+i*spacing*normDir.z);
		//System.out.println("planepoint: "+planePoint);

		Plane plane(viewDir*-1, planePoint);//have to reverse the view direction for the normal
		ProxyPiece proxyPiece=this->createSingleProxyPiece(cube, plane);
		if(proxyPiece.hasCorners()){
			allProxyPieces.push_back(proxyPiece);//add proxy piece to the list
		}
	}
	//for each point where there should be a plane
	//construct a plane at the point using the viewdir to get the normal--
	//use the cube and the plane to construct a proxy piece--
	//use the above method to find the intersections
	//add it to the collection of proxy pieces

	//each time the camera changes view directions, this method
	//needs to update the proxy piece locations
	//the proxy pieces should be contained by the upper level
	//the upper level will call their draw method

	//tomorrow get original class to use this one
	return allProxyPieces;
}

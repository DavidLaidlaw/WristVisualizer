#include "StdAfx.h"
#include "ProxyPiece.h"
#include <Windows.h>
#include "x64\glew-1.9.0\include\GL\glew.h"
#include <gl\GL.h>

ProxyPiece::ProxyPiece(const Plane& p, vector<Vector3>& co,const CubeGeometry& c):cube(c),corners(co),plane(p)
{

	if(this->hasCorners()){
			sortPointsInClockwiseOrderAroundAverageCenter();
		}

}


ProxyPiece::~ProxyPiece(void)
{
}




bool ProxyPiece::hasCorners(){
	//if it has 3 or more corners, then it does have corners
	if(corners.size()>=3){
		return true;
	}
	//otherwise it does not constituate a shape
	else{
		return false;
	}
}

float ProxyPiece::comp(Vector3 c, Vector3 a, Vector3 b){
	Vector3 AC=Vector3(a.x-c.x,a.y-c.y,a.z-c.z);
	Vector3 BC=Vector3(b.x-c.x,b.y-c.y,b.z-c.z);
	Vector3 cross=AC.cross( BC);
	return plane.getNormal().dot( cross);
}

void ProxyPiece::sortPointsInClockwiseOrderAroundAverageCenter(){
	//project points onto to defining plane, then sort those points

	//normal n, center C, points A and B
	//dot(n, cross(A-C, B-C)).
	//If the result is positive, B is counterclockwise from A;
	//if it's negative, B is clockwise from A.

	//creating a center vector
	float x=0;
	float y=0;
	float z=0;
	for (int i=0;i<corners.size();i++){
		Vector3 p=corners.at(i);
		//there may or may not be a null pointer here?
		x=x+p.x;
		y=y+p.y;
		z=z+p.z;
	}

	float n=corners.size();
	Vector3 center(x/n,y/n,z/n);
	vector<Vector3> newList;

	for (int j=0;j<n;j++){
		Vector3 p=corners.at(j);

		if(j==0){
			//if there are no people in the newList, add one
			vector<Vector3>::iterator it;
			 it = newList.begin();
			 it=newList.insert(it,p);
			//insert it at the beginning
		}
		else{
			for(int i=0;i<newList.size();i++){
				vector<Vector3>::iterator it;
				it = newList.begin();

				Vector3 b=newList.at(i);

				float det=this->comp(center, p, b);

				if (det<0){
					//insert new point before current point
					//test if it fits bewteen this and next
					it+=(i);//at the ith element
					//insert it at the current position
					newList.insert(it, p);
					break;
				}
				else if(i>=newList.size()-1){
					//end() points to the PAST THE END ELEMENT of the vector
					newList.insert(newList.end(), p);
					break;
				}
			}
		}

	}

	//the corners should equal the newly sorted list
	corners=newList;
}

void ProxyPiece::draw(bool isLeft){
	//just draw polys for now
	glPolygonMode( GL_FRONT_AND_BACK, GL_FILL);
			glBegin(GL_POLYGON);
			glColor3f(1.0f,  0.5f, 0.0f );
			for(int i=0;i<corners.size(); i++){
				//transform corners to local coordinates first? These might be world coordinates
				Vector3 texCoord=cube.convertModelToTextureCoord(corners.at(i));
				float xVal=texCoord.x;
				if(isLeft){
					xVal=1.0-xVal;
				}
				glTexCoord3f(xVal, texCoord.y,texCoord.z); 
				//glColor4f(texCoord.x, texCoord.y,texCoord.z,0.2f);
				
				/*if (isLeft){
					xVal=-xVal;
				}*/
				glVertex3f(corners.at(i).x, corners.at(i).y,corners.at(i).z );
			}
			glEnd();		
}
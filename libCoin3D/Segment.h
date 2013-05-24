#pragma once
#include "Vector.h"
class Segment
{
public:
	Segment(Vector3 s, Vector3 e);
	Segment();
	virtual ~Segment(void);

	Vector3 getStart(){
		return start;
	}

	Vector3 getEnd(){
		return end;
	}

private:
	Vector3 start;
	Vector3 end;
};


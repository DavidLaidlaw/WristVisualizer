#include "StdAfx.h"
#include "Segment.h"


Segment::Segment(Vector3 s, Vector3 e)
{
	start=s;
	end=e;
}

Segment::Segment()
{
	start=Vector3(-1,0,0);
	end=Vector3(-2,0,0);
}

Segment::~Segment(void)
{
}

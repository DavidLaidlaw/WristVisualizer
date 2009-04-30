#include "StdAfx.h"
#include "Node.h"

#include <Inventor/actions/SoWriteAction.h>
#include <Inventor/SoOutput.h>

libCoin3D::Node::Node(void)
{
}

static void* buffer_realloc(void * bufptr, size_t size)
{
	char* buffer = (char *)realloc(bufptr, size);
	return buffer;
}

System::String^ libCoin3D::Node::getNodeGraph()
{
	SoNode* node = this->getNode();
	if (node==NULL)
		return "";

	size_t startSize = 1024;
	char* buffer = (char*)malloc(startSize);
	SoOutput out;
	out.setBuffer(buffer,startSize,buffer_realloc);
	out.setBinary(FALSE); //save in ASCII format
	SoWriteAction wa(&out);

	wa.apply(node);

	size_t endPosition;
	void* outBuf = buffer;
	out.getBuffer(outBuf, endPosition);
	SbString s((const char*)outBuf);
	free(outBuf);
	return gcnew System::String(s.getString());
}


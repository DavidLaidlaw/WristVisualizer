#ifndef SIMVOLEON_CVRNONINDEXEDSETRENDERBASEP_H
#define SIMVOLEON_CVRNONINDEXEDSETRENDERBASEP_H

/**************************************************************************\
 * Copyright (c) Kongsberg Oil & Gas Technologies AS
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are
 * met:
 * 
 * Redistributions of source code must retain the above copyright notice,
 * this list of conditions and the following disclaimer.
 * 
 * Redistributions in binary form must reproduce the above copyright
 * notice, this list of conditions and the following disclaimer in the
 * documentation and/or other materials provided with the distribution.
 * 
 * Neither the name of the copyright holder nor the names of its
 * contributors may be used to endorse or promote products derived from
 * this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
\**************************************************************************/

#include <Inventor/SbBasic.h>

class SbVec3f;
class SoGLRenderAction;
class SoCoordinateElement;
class SoNonIndexedShape;
class SoMFInt32;
class SoState;

class Cvr3DTexCube;
class CvrCLUT;

// *************************************************************************

class CvrNonIndexedSetRenderBaseP {

public:
  virtual void getVertexData(SoState *state, 
                             const SoCoordinateElement *&coords, 
                             const SbVec3f *&normals, 
                             const SbBool neednormals) = 0;
  
  void GLRender(SoGLRenderAction * action, 
                const float offset,
                const SbBool clipGeometry,
                SoMFInt32 & numVertices);
  
  enum SetType { FACESET, TRIANGLESTRIPSET };

  Cvr3DTexCube * cube;
  const CvrCLUT * clut;
  uint32_t parentnodeid;
  SoNonIndexedShape * clipgeometryshape; 
  enum SetType type;

protected:
  SoNonIndexedShape * master; 
};

// *************************************************************************

#endif // !SIMVOLEON_CVRNONINDEXEDSETRENDERBASEP_H
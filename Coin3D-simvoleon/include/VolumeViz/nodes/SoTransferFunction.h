#ifndef COIN_SOTRANSFERFUNCTION_H
#define COIN_SOTRANSFERFUNCTION_H

/**************************************************************************\
 *
 *  This file is part of the SIM Voleon visualization library.
 *  Copyright (C) 2003-2004 by Systems in Motion.  All rights reserved.
 *
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU General Public License
 *  ("GPL") version 2 as published by the Free Software Foundation.
 *  See the file LICENSE.GPL at the root directory of this source
 *  distribution for additional information about the GNU GPL.
 *
 *  For using SIM Voleon with software that can not be combined with
 *  the GNU GPL, and for taking advantage of the additional benefits
 *  of our support services, please contact Systems in Motion about
 *  acquiring a SIM Voleon Professional Edition License.
 *
 *  See <URL:http://www.coin3d.org/> for more information.
 *
 *  Systems in Motion, Postboks 1283, Pirsenteret, 7462 Trondheim, NORWAY.
 *  <URL:http://www.sim.no/>.
 *
\**************************************************************************/

#include <VolumeViz/nodes/SoVolumeRendering.h>
#include <Inventor/fields/SoMFFloat.h>
#include <Inventor/fields/SoSFEnum.h>
#include <Inventor/fields/SoSFInt32.h>
#include <Inventor/fields/SoSFUInt32.h>

// *************************************************************************

class SIMVOLEON_DLL_API SoTransferFunction : public SoVolumeRendering {
  typedef SoVolumeRendering inherited;

  SO_NODE_HEADER(SoTransferFunction);

public:
  static void initClass(void);
  SoTransferFunction(void);

  enum PredefColorMap {
    NONE = 0,
    GREY,
    GRAY = GREY,
    TEMPERATURE,
    PHYSICS,
    STANDARD,
    GLOW,
    BLUE_RED,
    SEISMIC
  };

  enum ColorMapType {
    ALPHA,
    LUM_ALPHA,
    RGBA
  };

  SoSFInt32 shift;
  SoSFInt32 offset;
  SoSFEnum predefColorMap;
  SoSFEnum colorMapType;
  SoMFFloat colorMap;

  void reMap(int low, int high);
  SbBool hasTransparency(void) const;

protected:
  ~SoTransferFunction();

  virtual void doAction(SoAction * action);
  virtual void GLRender(SoGLRenderAction * action);
  virtual void callback(SoCallbackAction * action);
  virtual void pick(SoPickAction * action);

private:
  // These were added to make it possible to control the narrowing of
  // the transfer function from the iv-file. They provide the same
  // functionality as the reMap() function.
  //
  // These are "unofficial", private fields, as they are not available
  // in TGS VolumeViz, which we want to stay compatible with.
  SoSFUInt32 remapLow;
  SoSFUInt32 remapHigh;

  friend class SoTransferFunctionP;
  class SoTransferFunctionP * pimpl;
};

// *************************************************************************

#endif // !COIN_SOTRANSFERFUNCTION_H
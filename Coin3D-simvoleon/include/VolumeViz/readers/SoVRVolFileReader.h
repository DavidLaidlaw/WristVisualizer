#ifndef COIN_SOVRVOLFILEREADER_H
#define COIN_SOVRVOLFILEREADER_H

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

#include <VolumeViz/readers/SoVolumeReader.h>


class SIMVOLEON_DLL_API SoVRVolFileReader : public SoVolumeReader {
  typedef SoVolumeReader inherited;

public:
  SoVRVolFileReader(void);
  ~SoVRVolFileReader();

  void setUserData(void * data);
  void getDataChar(SbBox3f & size, SoVolumeData::DataType & type, SbVec3s & dim);
  virtual void getSubSlice(SbBox2s & subslice, int slicenumber, void * data);

private:
  class SoVRVolFileReaderP * pimpl;
  friend class SoVRVolFileReaderP;
};

#endif // ! COIN_SOVRVOLFILEREADER_H
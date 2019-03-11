#pragma once
#ifndef COIN_GLPERFORMANCE_H
#define COIN_GLPERFORMANCE_H

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

#include <Inventor/C/glue/gl.h>
#include <Inventor/SbTime.h>

// *************************************************************************

/* List of all modules using this code, and under what prefix: */

#ifdef COIN_INTERNAL
#define COINSHAREPREFIX coin
#endif /* COIN_INTERNAL */

#ifdef SIMVOLEON_INTERNAL
#define COINSHAREPREFIX voleon
#endif /* SIMVOLEON_INTERNAL */

/* (We need different prefixes to avoid the possibility of namespace
   clashes for the linker. At least the OS X linker will complain of
   symbols defined multiple times, under its default configuration.)
*/

// *************************************************************************

#define CC_PERF_PRE_CB SO__CONCAT(COINSHAREPREFIX,_perf_pre_cb)
#define CC_PERF_RENDER_CB SO__CONCAT(COINSHAREPREFIX,_perf_render_cb)
#define CC_PERF_POST_CB SO__CONCAT(COINSHAREPREFIX,_perf_post_cb)

#define cc_perf_gl_timer SO__CONCAT(COINSHAREPREFIX,_perf_gl_timer)

#undef COINSHAREPREFIX

// *************************************************************************

typedef void CC_PERF_PRE_CB(const cc_glglue * g, void * userdata);
typedef void CC_PERF_RENDER_CB(const cc_glglue * g, void * userdata);
typedef void CC_PERF_POST_CB(const cc_glglue * g, void * userdata);

// *************************************************************************

/*
  Usage example from SIM Voleon:
  \code
	  #include <[...]/CoinGLPerformance.h>
	  // [...]
	  const CC_PERF_RENDER_CB * perfchkfuncs[2] = {
		SoVolumeRenderP::render2DTexturedTriangles,
		SoVolumeRenderP::render3DTexturedTriangles
	  };
	  double timings[2];

	  const SbTime t =
		cc_perf_gl_timer(glglue, 2, perfchkfuncs, timings,
						 SoVolumeRenderP::setupPerformanceTest,
						 SoVolumeRenderP::cleanupPerformanceTest);

	  const double rating = timings[1] / timings[0]; // 3d / 2d
	  if (rating < 10.0f) { // 2D should at least be this many times
							// faster before 3D texturing is dropped.
		do3dtextures = 1;
		return TRUE;
	  }
  \endcode
  Render the stuff approximately at origo, as the camera is set up
  before invoking the user-supplied render callback as a perspective
  camera at <0, 0, -0.5>, 45° vertical field-of-view, near plane at
  0.1 and far plane at 10.0.
  An important note about the usage: be aware that this is not meant
  to be used for measuring cases where there are small differences in
  performance between competing techiques due to e.g. better pipeline
  parallelization or other such, rather marginal, effects.
  This is supposed to be used to smoke out systems/drivers where there
  are *major* performance hits suffered for OpenGL techniques that are
  available, but not optimal for use. This can for instance happen if
  a technique is available in the driver, but is not hardware
  accelerated with the particular hardware.  Case in point: 3D
  texturing is available in all OpenGL 1.2+ drivers, but often not
  hardware accelerated on many systems.  Or perhaps the GL feature we
  want to measure / check is just known to be really badly implemented
  on certain systems, or to have one or more bugs which causes major
  slowdowns on other systems.
*/

const SbTime
cc_perf_gl_timer(const cc_glglue * glue,
	const unsigned int nrrendercbs,
	CC_PERF_RENDER_CB * rendercbs[],
	double averagerendertime[],

	CC_PERF_PRE_CB * precb = NULL,
	CC_PERF_POST_CB * postcb = NULL,
	const unsigned int maxruns = 10,
	const SbTime maxtime = SbTime(0.5),
	void * userdata = NULL);

// *************************************************************************

#endif // ! COIN_GLPERFORMANCE_H

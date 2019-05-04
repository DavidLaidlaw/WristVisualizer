# Building and Installing External Packages in Support of
WristVisualizer 

The WristVisualizer application is based on the open
source Open Inventor (Coin3D) and it's ancillary support package SoWin
that permits Coin3D to interact with the Windows OS.

## Build Instructions For Windows

To build WristVisualier on Windows, the following packages have be installed first:
1- Coin3D
2- SoWin

### Coin3D

Download Coin3D 3.1.3 from
[here] (https://bitbucket.org/Coin3D/coin/downloads/). Follow build
[instructions] (https://bitbucket.org/Coin3D/coin/wiki/Building%20instructions). Synoptically:

-Define the environment variable COINDIR as detailed in the directions (don't forget to create it).  

-Install the source on you local disk as discussed in the directions.

-Open the Visual Studio coin3d.sln Solution for in the build/msvc10 directory found under your source root path

-Retarget your SDKs (right click on coin3d.sln, select retarget and recommended SDK value) 

-Build coin3d.sln first, then 

-Build coin3d-install.sln

### SoWin

SoWin 1.5  is built in the same manner as Coin3d. [Download] (https://bitbucket.org/Coin3D/coin/downloads/).

- Set the SOWINDIR environment variable as in the case of Coin3D.

-Open the Visual Studio coin3d.sln Solution for in the build/msvc9 directory found under your source root path

-Retarget your SDKs

-Build and Install following similar steps as in Coin3d.


### WristVisualier

Open Visual studio solution visualiation.sln. 


Build the solution.  The executable is placed in the WristViualier subdirectory in bin/x64/Debug/WristVizualizer.exe.

# WristVisualizer

This repository contains 
- source code for the WristVisualizer, and;
- installable executables (for Windows). 

The source code  compiles with Visual Studio.  The latest version of
the code builds with Visual Studio 2017 and above.  

## Source 
The repository contains 4 versions of the source each in a separate branch:

- **VS2010 branch**.  Builds in Visual Studio 2010.  This is an older
 version. It is not fully self contained.  It requires separate
 download and installation of the Coin3D and SoWin packages. See
 [Install External Packages](./InstallExternalPackages.md). It also
 lacks volume rendering and the latest enhancements.  No further
 development is planned for this version.

- **VS2017 branch**. This is the version of the VS2010 code that
 builds in VS2017 and above.  No further development is planned for
 this version. This version has no changes since Jan. 2019. See
 [Install External Packages](./InstallExternalPackages.md) to build required external
 packages.


- **VS2017_with_Coin3D branch**. This is similar to the VS2017 branch
  but includes Coin3D and SoWin packages. Volume rendering is missing from
  this version. Latest user interface enhancements are not included.  No
  further enhancements are planned for this version as of March 2019.

- **master**.  The lastest code with Coin3D, SoWin, Volume rendering
  and latest UI changes. Any future additions are planned for this
  branch. If in doubt, use this branch.

## Getting WristVisualizer

The WristVisualizer application can either be built locally from
source or installed from the latest pre-built version using the
Window's installer.

For either option, fetch the source by either cloning the repository or downloading it (on Github, see green *Clone or Download* button ). 

### Installing 

Download the source first to gain access to the SetupInstaller directory (subproject). SetupInstaller is integrated with the WristVisualizer solution.  It builds a Windows  installer script called “SetupInstaller.msi” in *WristVisualizer/SetupInstaller/SetupInstaller-SetupFiles/SetupInstaller.msi* which can be double-clicked from Windows or executed on the command line with either:
1.  *msiexec.exe –i SetupInstaller.msi* : does a normal installation in *C:/Program Files/* and add WristVisualizer in the normal programs menu
2.  *msiexec.exe –a SetupInstaller.msi* : uncompresses all need files in the current directory so that WristVisualizer can be executed in that directory without writing to *c:/Program Files* 

### Building from Source

To build from source, download the source as described above. Then,
open the *WristVisualizer/Visualization.sln* solution file with VS2017
(or higher). The solution file contains 8 projects. Build 
Visualization.sln.  Remember to choose the build platform. Two choices
have been tested : *x64 Debug* and *x64 Release*.  Depending on your
target platform, the executable will be placed in a subdirectory of
*WristVisualizer/bin* e.g. *bin/x64/Debug/WristVizualizer.exe*.


### Running WristVisualizer

The distribution contains example data which could be used to test the
application.  The directory
*WristVisualizer/SetupWristVizualizer/Sample Wrist* has a example full
wrist that can be opened using File>Open Full Wrist.


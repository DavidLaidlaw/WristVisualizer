### Building the Microsoft Installer Project in WristVisualizer

One of the projects within the WristVisualizer solution is a Microsoft
Visual Studio Installer (Setup) project.  Currently, WristVisualizer
is not set up to build the Microsoft Installer project (named
"Microsoft Installer") because, on x64 Windows machines, this project can
"hang" when being built.  

To resolve this behavior before building Microsoft Installer :

1. Open a cmd window as Administrator.
2. Run: ***regsvr32.exe /u "C:\Program Files (x86)\Common Files\microsoft shared\MSI Tools\mergemod.dll"***
3. Then run: ***regsvr32.exe "C:\Program Files (x86)\Common Files\microsoft shared\MSI Tools\mergemod.dll"***

You should be now able to build the installer project. 

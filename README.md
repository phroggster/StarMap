<!-- [![Build status](https://ci.appveyor.com/api/projects/status/lek706212qngk600?svg=true)](https://ci.appveyor.com/project/phroggie/starmap) -->

## **This repository has been archived to history. No future updates will be forthcoming.**

# StarMap
An OpenGL 4.5 ForwardCompatible (shaders are `#version 450 core`) ~~fork~~ younger sibling of the [EDDiscovery][1] 3d map viewer.

This project is currently in its infancy (read: ***PRE-ALPHA***). Please bear with me while I help it to learn how to crawl. Supervised play dates (pull requests) [are welcome][2]!

### Base Requirements
* .NET Framework [4.6][3] (included in Windows 10; [4.6.2][4] recommended).
* An [EDDiscovery][1] systems database available at `%LOCALAPPDATA%\EDDiscovery\EDDSystem.sqlite`. (Check out [StarMap.Database.ADBConnection.GetSQLiteDBFile][8] to modify)
* A graphics card that supports the OpenGL v4.5 API (GeForce 400+, Radeon HD 7000+, Intel Broadwell+), with recent drivers.

### Build Requirements
Same as [Base Requirements](#base-requirements), plus:
* ~~.NET Framework [2.0/3.5][5] (to compile the included GLControl).~~ (Now split off to [phrogGLControl][6])
* Visual Studio 2015 (2017 is untested, but *should* also work; quite possibly others).

### Download
At least until StarMap enters the Alpha development stage, you can either build StarMap yourself or grab a recent portable build from [AppVeyor][7].

 [1]:https://github.com/EDDiscovery/EDDiscovery
 [2]:https://github.com/phroggster/StarMap/pulls
 [3]:http://go.microsoft.com/fwlink/?LinkId=528259
 [4]:http://go.microsoft.com/fwlink/?LinkId=780597
 [5]:https://answers.microsoft.com/en-us/insider/forum/insider_wintp-insider_install/how-to-instal-net-framework-35-on-windows-10/450b3ba6-4d19-45ae-840e-78519f36d7a4?auth=1
 [6]:https://github.com/phroggster/phrogGLControl
 [7]:https://ci.appveyor.com/project/phroggie/starmap/build/artifacts
 [8]:https://github.com/phroggster/StarMap/blob/master/StarMap/Database/ADBConnection.cs#L268

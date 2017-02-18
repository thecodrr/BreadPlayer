:: Name:     msbuild.cmd
:: Purpose:  Automates the build and install process for BreadPlayer.
:: Author:   theweavr (enkaboot@gmail.com)
:: Revision: January 2016 - initial version

echo off
SET BP_VERSION=1.1.0.0
title Building BreadPlayer
echo Thank you for choosing Bread!
echo Adding MSBUILD to PATH...
rem Add path to MSBuild Binaries
if exist "%ProgramFiles%\MSBuild\14.0\bin" set PATH=%ProgramFiles%\MSBuild\14.0\bin;%PATH%
if exist "%ProgramFiles(x86)%\MSBuild\14.0\bin" set PATH=%ProgramFiles(x86)%\MSBuild\14.0\bin;%PATH%
echo Added MSBUILD to PATH...
echo Restoring Nuget Packages...
nuget.exe restore ../BreadPlayer.NoTests.sln
echo Nuget Packages Successfully Restored...
echo Building...
msbuild.exe ../BreadPlayer.NoTests.sln /p:Configuration=Release /p:Platform="x64"
if exist "BreadPlayer.Views.UWP\AppPackages\BreadPlayer.Views.UWP_%BP_VERSION%_Test" (
	echo Building Complete
	title Installing BreadPlayer
	echo Installing BreadPlayer...
	echo Entering Powershell...
	powershell "../BreadPlayer.Views.UWP\AppPackages\BreadPlayer.Views.UWP_%BP_VERSION%_Test\Add-AppDevPackage.ps1"
	echo BreadPlayer Installed!
	title Thank you for installing Bread Player!
	echo Welcome to the Bread Player Community!
	)else ( 
 	 echo Build failed...
 	 echo Exiting...
	)
pause
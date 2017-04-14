echo off
setlocal
set NOPAUSE=true
:: Set the Source Directory
set SOURCE=%~dp0

:: Set the Visual Studio Install Directory
set INSTALL_LOCATION=%~1
if not defined INSTALL_LOCATION set INSTALL_LOCATION=%VSINSTALLDIR%

set MSBUILD_APPXPACKAGE_DIR='C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\Microsoft\VisualStudio\v15.0\AppxPackage'
echo The Microsoft.Appxpackage.Targets file will be installed in the following directory: 'C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\Microsoft\VisualStudio\v15.0\AppxPackage'
 
:: Make a backup of the .targets file
::
echo.
echo Backing up the Microsoft.Appxpackage.Targets file...
echo.
copy /y "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\Microsoft\VisualStudio\v15.0\AppxPackage\Microsoft.AppxPackage.Targets" "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\Microsoft\VisualStudio\v15.0\AppxPackage\Microsoft.AppxPackage.Targets-BAK"


:: Patch AppxPackage tasks/targets
::
echo.
echo Copying the Microsoft.AppxPackage.Targets from %SOURCE% to %MSBUILD_APPXPACKAGE_DIR%...
echo.
copy /y %SOURCE%Microsoft.AppxPackage.Targets "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\Microsoft\VisualStudio\v15.0\AppxPackage\Microsoft.AppxPackage.Targets"

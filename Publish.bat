@echo off
setlocal


REM Set the path to the .NET SDK and msbuild <:)

set dotnetPath=dotnet
set msbuildPath="%ProgramFiles%\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\msbuild.exe"


REM Set stuff about the loader project

set loaderProjectName=BananaModManager.LoaderCompiler

set loaderConfiguration=Release
set loaderPlatform=AnyCPU

set loaderOutputFolder=%loaderProjectName%\bin\%loaderConfiguration%
set loaderDestinationFolder=Publish\Loader


REM Do the same with the NewUI project

set newUIProjectName=BananaModManager.NewUI

set newUIConfiguration=Release
set newUIPlatform=x86
set newUIRuntime=win-x86
set newUIFramework=net6.0-windows10.0.19041.0
set newUIIsSelfContained=true
set newUITrimUnusedAssemblies=true

set newUIOutputFolder=%newUIProjectName%\bin\%newUIConfiguration%
set newUIDestinationFolder=Publish


REM Set Detours project details

set detoursProjectName=BananaModManager.Detours

set detoursConfiguration=Release
set detoursPlatform64=x64
set detoursPlatform86=x86
set detoursOutputFolder=%detoursProjectName%\Release
set detoursDestinationFolder64=Publish\Loader\x64
set detoursDestinationFolder86=Publish\Loader\x86


REM ==========================================================================================================================================


REM Ask the user which projects to compile the projects

echo Do you want to compile %newUIProjectName%? (Y/N)
set /p compileNewUI=
echo Do you want to compile %loaderProjectName%? (Y/N)
set /p compileLoader=
echo Do you want to compile %detoursProjectName%? (Y/N)
set /p compileDetours=

if /i "%compileNewUI%" neq "Y" goto SkipNewUICompilation


REM ==========================================================================================================================================



REM Build the NewUI project

echo Compiling %newUIProjectName%...
%dotnetPath% restore "%newUIProjectName%\%newUIProjectName%.csproj"
%dotnetPath% publish "%newUIProjectName%\%newUIProjectName%.csproj" -c %newUIConfiguration% -p:Platform=%newUIPlatform% -o %newUIDestinationFolder% --self-contained %newUIIsSelfContained% --runtime %newUIRuntime% --framework %newUIFramework% -p:PublishTrimmed=%newUITrimUnusedAssemblies%


REM Check if the build was successful

if %errorlevel% neq 0 (
    echo %newUIProjectName% compilation failed. Exiting.
    exit /b %errorlevel%
)


REM Copy all the files to the destination folder

echo Copying files to %newUIDestinationFolder%...
xcopy /Y /E /I /Q /EXCLUDE:PublishExclude.txt "%newUIProjectName%\bin\%newUIConfiguration%\%newUIPlatform%\%newUIFramework%\%newUIRuntime%\*" "%newUIDestinationFolder%"



REM ==========================================================================================================================================


:SkipNewUICompilation

if /i "%compileLoader%" neq "Y" goto SkipLoaderCompilation


REM ==========================================================================================================================================



REM Build the LoaderCompiler project

echo Compiling %loaderProjectName%...
%dotnetPath% restore "%loaderProjectName%\%loaderProjectName%.csproj"
%dotnetPath% build "%loaderProjectName%\%loaderProjectName%.csproj" -c %loaderConfiguration% -p:Platform=%loaderPlatform%


REM Check if the build was successful

if %errorlevel% neq 0 (
    echo %loaderProjectName% compilation failed. Exiting.
    exit /b %errorlevel%
)


REM Create the destination folder if it doesn't exist

if not exist %loaderDestinationFolder% (
    mkdir %loaderDestinationFolder%
)


REM Copy the required files to the Loader folder

echo Copying files to %loaderDestinationFolder%...
copy /Y "%loaderOutputFolder%\BananaModManager.Shared.dll" "%loaderDestinationFolder%"
copy /Y "%loaderOutputFolder%\BananaModManager.Loader.IL2Cpp.dll" "%loaderDestinationFolder%"
copy /Y "%loaderOutputFolder%\BananaModManager.Loader.Mono.dll" "%loaderDestinationFolder%"
copy /Y "%loaderOutputFolder%\DiscordRPC.dll" "%loaderDestinationFolder%"
copy /Y "%loaderOutputFolder%\System.Text.Json.dll" "%loaderDestinationFolder%"
copy /Y "%loaderOutputFolder%\System.Memory.dll" "%loaderDestinationFolder%"



REM ==========================================================================================================================================


:SkipLoaderCompilation

if /i "%compileDetours%" neq "Y" goto SkipDetoursCompilation


REM ==========================================================================================================================================



REM Build the Detours project

echo Compiling %detoursProjectName%...
cd %detoursProjectName%
%msbuildPath% /p:Configuration=%detoursConfiguration%;Platform=%detoursPlatform64%
cd ..


REM Check if the build was successful

if %errorlevel% neq 0 (
    echo %detoursProjectName% compilation failed. Exiting.
    exit /b %errorlevel%
)


REM Create the destination folder if it doesn't exist

if not exist %detoursDestinationFolder64% (
    mkdir %detoursDestinationFolder64%
)

REM Copy the DLL to the Loader folder

copy /Y "%detoursOutputFolder%\%detoursProjectName%.dll" "%detoursDestinationFolder64%"


REM Build again but for x86

echo Compiling %detoursProjectName% again!
cd %detoursProjectName%
%msbuildPath% /p:Configuration=%detoursConfiguration%;Platform=%detoursPlatform86%
cd ..


REM Check if the build was successful

if %errorlevel% neq 0 (
    echo %detoursProjectName% compilation failed. Exiting.
    exit /b %errorlevel%
)


REM Create the destination folder if it doesn't exist

if not exist %detoursDestinationFolder86% (
    mkdir %detoursDestinationFolder86%
)

REM Copy the DLL to the Loader folder

copy /Y "%detoursOutputFolder%\%detoursProjectName%.dll" "%detoursDestinationFolder86%"



REM ==========================================================================================================================================


:SkipDetoursCompilation

echo. 
echo Ding ding ding! We are done!

REM %SystemRoot%\explorer.exe "%loaderDestinationFolder%"

set /P "=Press any key to continue... " <nul
pause >nul

exit /b 0
@echo off

set _programFiles=%ProgramFiles(x86)%
if not defined _programFiles set _programFiles=%ProgramFiles%

set _msbuildPath=%_programFiles%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin
set _roslynatorPath=..\..\Roslynator\src

"%_msbuildPath%\msbuild" "%_roslynatorPath%\CommandLine.sln" /t:Build /p:Configuration=Release /v:m /m

"%_roslynatorPath%\CommandLine\bin\Release\net48\roslynator" loc "..\src\Orang.sln" ^
 --msbuild-path "%_msbuildPath%" ^
 --ignore-block-boundary ^
 --verbosity d

pause

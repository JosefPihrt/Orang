@echo off

set _msbuildPath=C:\Program Files\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin
set _roslynatorPath=..\..\Roslynator\src

"%_msbuildPath%\msbuild" "%_roslynatorPath%\CommandLine.sln" /t:Build /p:Configuration=Release /v:m /m

"%_roslynatorPath%\CommandLine\bin\Release\net48\roslynator" loc "..\src\Orang.sln" ^
 --msbuild-path "%_msbuildPath%" ^
 --ignore-block-boundary ^
 --verbosity d

pause

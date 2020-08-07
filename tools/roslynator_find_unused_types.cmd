@echo off

set _roslynatorPath=..\..\Roslynator\src
set _msbuildPath="C:\Program Files\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin"

%_msbuildPath%\msbuild "%_roslynatorPath%\CommandLine.sln" /t:Build /p:Configuration=Debug /v:m /m

"%_roslynatorPath%\CommandLine\bin\Debug\net48\roslynator" find-symbols "..\src\Orang.sln" ^
 --msbuild-path %_msbuildPath% ^
 --visibility public internal private ^
 --symbol-groups type ^
 --without-attributes ^
  "System.ObsoleteAttribute" ^
 --ignored-projects CommandLine.Tests ^
 --unused-only ^
 --verbosity n ^
 --file-log "roslynator.log" ^
 --file-log-verbosity diag

pause

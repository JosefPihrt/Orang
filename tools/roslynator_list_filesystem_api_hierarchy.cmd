@echo off

set _roslynatorPath=..\..\Roslynator\src
set _msbuildPath="C:\Program Files\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin"

%_msbuildPath%\msbuild "%_roslynatorPath%\CommandLine.sln" /t:Build /p:Configuration=Debug /v:m /m

"%_roslynatorPath%\CommandLine\bin\Debug\net48\roslynator" list-symbols "..\src\FileSystem\FileSystem.csproj" ^
 --msbuild-path %_msbuildPath% ^
 --depth type ^
 --visibility public ^
 --layout type-hierarchy ^
 --ignored-parts assemblies containing-namespace-in-type-hierarchy assembly-attributes accessibility base-type constraints ^
 --output orang_filesystem_api_hierarchy.txt ^
 --verbosity d ^
 --file-log "roslynator.log" ^
 --file-log-verbosity diag

pause

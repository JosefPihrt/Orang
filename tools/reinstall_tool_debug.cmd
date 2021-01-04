@echo off

set _programFiles=%ProgramFiles(x86)%
if not defined _programFiles set _programFiles=%ProgramFiles%

"%_programFiles%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\msbuild" "..\src\CommandLine\CommandLine.csproj" ^
 /t:Clean,Build ^
 /p:Configuration=Debug,RunCodeAnalysis=false ^
 /nr:false ^
 /v:normal ^
 /m

if errorlevel 1 (
 pause
 exit
)

dotnet pack -c Debug --no-build -v normal "..\src\CommandLine\CommandLine.csproj"

dotnet tool uninstall orang.dotnet.cli -g

dotnet tool install orang.dotnet.cli --version 0.2.0-rc -g --add-source "..\src\CommandLine\bin\Debug"

pause
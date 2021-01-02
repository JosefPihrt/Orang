@echo off

set _programFiles=%ProgramFiles(x86)%
if not defined _programFiles set _programFiles=%ProgramFiles%

set _msbuildPath="%_programFiles%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild"
set _properties=Configuration=Debug,Deterministic=true

%_msbuildPath% "..\src\DocumentationGenerator\DocumentationGenerator.csproj" ^
 /t:Build ^
 /p:%_properties% ^
 /v:minimal ^
 /m

if errorlevel 1 (
 pause
 exit
)

dotnet "..\src\DocumentationGenerator\bin\Debug\netcoreapp3.1\Orang.DocumentationGenerator.dll" "..\docs\cli"

if errorlevel 1 (
 pause
 exit
)

echo OK
pause

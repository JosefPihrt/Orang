@echo off

set _programFiles=%ProgramFiles(x86)%
if not defined _programFiles set _programFiles=%ProgramFiles%

set _version=0.2.1

orang replace -e cmd -c "(?<=--version )\d+\.\d+\.\d+(-\w+)?" -r "%_version%"

orang replace "..\src" -e csproj -c "(?<=<PackageVersion>)\d+\.\d+\.\d+(-\w+)?(?=</PackageVersion>)" -r "%_version%"

orang replace "..\src\CommandLine\PackageInfo.cs" -c "(?<="")\d+\.\d+\.\d+(-\w+)?(?="")" -r "%_version%"

echo.

orang delete "..\src" -a d -n "bin|obj" e --content-only -t n -y su s

echo.

dotnet restore --force "..\src\Orang.sln"

"%_programFiles%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\msbuild" "..\src\Orang.sln" ^
 /t:Clean,Build ^
 /p:Configuration=Release,RunCodeAnalysis=false,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors=1591 ^
 /nr:false ^
 /v:normal ^
 /m

if errorlevel 1 (
 pause
 exit
)

dotnet "..\src\DocumentationGenerator\bin\Release\netcoreapp3.1\Orang.DocumentationGenerator.dll" "..\docs\cli"

if errorlevel 1 (
 pause
 exit
)

dotnet "..\src\CommandLine\bin\Release\netcoreapp3.1\Orang.dll" help -m -v d > "..\docs\cli\manual.txt"

if errorlevel 1 (
 pause
 exit
)

dotnet test -c Release --no-build "..\src\Tests\CommandLine.Tests\CommandLine.Tests.csproj"

if errorlevel 1 (
 pause
 exit
)

dotnet pack -c Release --no-build -v normal "..\src\CommandLine\CommandLine.csproj"

set _outDir=..\out\Release

md "%_outDir%"
del /Q "%_outDir%\*"
copy "..\src\CommandLine\bin\Release\Orang.DotNet.Cli.*.nupkg" "%_outDir%"

pause
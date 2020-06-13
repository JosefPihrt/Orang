@echo off

dotnet restore --force "..\src\Orang.sln"

"%ProgramFiles%\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\msbuild" "..\src\Orang.sln" ^
 /t:Clean,Build ^
 /p:Configuration=Release,RunCodeAnalysis=false,Deterministic=true,TreatWarningsAsErrors=true,WarningsNotAsErrors=1591 ^
 /nr:false ^
 /v:normal ^
 /m

if errorlevel 1 (
 pause
 exit
)

dotnet "..\src\DocumentationGenerator\bin\Release\netcoreapp3.0\Orang.DocumentationGenerator.dll" "..\docs\cli"

if errorlevel 1 (
 pause
 exit
)

dotnet "..\src\CommandLine\bin\Release\netcoreapp3.0\Orang.dll" help -m -v d > "..\docs\cli\manual.txt"

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

pause
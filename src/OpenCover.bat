@echo off

set PROJECT=CoreUpdater.Tests

set OPENCOVER="%USERPROFILE%\.nuget\packages\opencover\4.6.519\tools\OpenCover.Console.exe"

rem Test command
set TARGET=dotnet.exe

rem Test command argument
set TARGET_TEST="test %~dp0%PROJECT%\%PROJECT%.csproj"

rem OpenCover output
set OUTPUT=OpenCover.xml

set FILTERS="+[*]*"

%OPENCOVER% -register:user -target:%TARGET% -targetargs:%TARGET_TEST% -filter:%FILTERS% -oldstyle -output:%~dp0%OUTPUT%
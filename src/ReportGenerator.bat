@echo off

set REPORTGEN="%USERPROFILE%\.nuget\packages\ReportGenerator\3.1.2\tools\ReportGenerator.exe"

rem OpenCover output
set OUTPUT="OpenCover.xml"

rem ReportGenerator output
set OUTPUT_DIR="ReportGenerator"

%REPORTGEN% -reports:%~dp0%OUTPUT% -targetdir:%~dp0%OUTPUT_DIR%
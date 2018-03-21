set REPORTGEN="%USERPROFILE%\.nuget\packages\ReportGenerator\3.1.2\tools\ReportGenerator.exe"

rem OpenCover output
set OUTPUT="OpenCover.xml"

rem ReportGenerator output
set OUTPUT_DIR="ReportGenerator"

%REPORTGEN% -reports:%OUTPUT% -targetdir:%OUTPUT_DIR%
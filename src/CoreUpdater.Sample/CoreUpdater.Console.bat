cd %~dp0
SET APP="..\CoreUpdater.Console\bin\Release\netcoreapp2.0\CoreUpdater.Console.dll"
dotnet %APP% -d %~dp0bin\Release\netcoreapp2.0 -n=CoreUpdater.Sample -v=2.0.0
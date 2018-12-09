#dotnet publish -c Release --self-contained --runtime win7-x86

# build server
cd src\User\RetroDbBlaze\RetroDbBlaze.Server
dotnet publish -c Release
XCOPY /I /E /Y  ".\bin\Release\netcoreapp2.1\publish\*.*" "..\..\..\..\Build\"

# build importer
cd ..\..\..\..
cd src\User\RetroDbImporter.Console
dotnet publish -c Release
XCOPY /I /E /Y  ".\bin\Release\netcoreapp2.1\publish\*.*" "..\..\..\Build\"
```bash

mkdir release/

pushd KeyVox.Engine.Cli/
dotnet publish -c Release -r win-x64 --self-contained -o ../release/
pushd ../

pushd OsSpecificImplementation/KeyVox.OsSpecific.Windows.App/
dotnet publish -c Release -r win-x64 --self-contained -o ../../release/
pushd ../../

./release/KeyVox.OsSpecific.Windows.App.exe

```
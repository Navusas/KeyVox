# Create the release directory
New-Item -Path . -Name "release" -ItemType "directory" -Force

# Navigate to KeyVox.Engine.Cli and publish
Push-Location -Path "KeyVox.Engine.Cli"
dotnet publish -c Release -r win-x64 --self-contained -o ../release/
Pop-Location

# Navigate to OsSpecificImplementation/KeyVox.OsSpecific.Windows.App and publish
Push-Location -Path "OsSpecificImplementation/KeyVox.OsSpecific.Windows.App"
dotnet publish -c Release -r win-x64 --self-contained -o ../../release/
Pop-Location

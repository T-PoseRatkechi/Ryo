# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

Remove-Item "$env:RELOADEDIIMODS/Ryo.Reloaded/*" -Force -Recurse
dotnet publish "./Ryo.Reloaded.csproj" -c Release -o "$env:RELOADEDIIMODS/Ryo.Reloaded" /p:OutputPath="./bin/Release" /p:ReloadedILLink="true"

# Restore Working Directory
Pop-Location
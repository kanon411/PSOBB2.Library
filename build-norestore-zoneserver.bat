dotnet publish src/server/PSOBB.Server.ZoneServer/PSOBB.Server.ZoneServer.csproj -c Release

if not exist "build\zoneserver" mkdir build\zoneserver

xcopy src\server\PSOBB.Server.ZoneServer\bin\Release\netstandard2.0\publish build\zoneserver /Y /q /EXCLUDE:BuildExclude.txt
EXIT 0
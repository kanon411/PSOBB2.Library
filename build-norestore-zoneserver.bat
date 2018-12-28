dotnet publish src/server/Guardians.Server.ZoneServer/Guardians.Server.ZoneServer.csproj -c Release

if not exist "build\zoneserver" mkdir build\zoneserver

xcopy src\server\Guardians.Server.ZoneServer\bin\Release\netstandard2.0\publish build\zoneserver /Y /q /EXCLUDE:BuildExclude.txt
EXIT 0
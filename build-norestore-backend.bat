dotnet publish src/Server/PSOBB.Service.Authentication/PSOBB.Service.Authentication.csproj -c DEBUG_LOCAL
dotnet publish src/Server/PSOBB.Service.ServiceDiscovery/PSOBB.Service.ServiceDiscovery.csproj -c DEBUG_LOCAL
dotnet publish src/Server/PSOBB.Service.ServerSelection/PSOBB.Service.ServerSelection.csproj -c DEBUG_LOCAL
dotnet publish src/Server/PSOBB.Service.GameServer/PSOBB.Service.GameServer.csproj -c DEBUG_LOCAL
dotnet publish src/Server/PSOBB.Service.ContentServer/PSOBB.Service.ContentServer.csproj -c DEBUG_LOCAL
dotnet publish src/Server/PSOBB.Service.Social/PSOBB.Service.Social.csproj -c DEBUG_LOCAL

if not exist "build\auth" mkdir build\auth
if not exist "build\servdisc" mkdir build\servdisc
if not exist "build\servsel" mkdir build\servsel
if not exist "build\gameservdisc" mkdir build\gameservdisc
if not exist "build\gameserv" mkdir build\gameserv
if not exist "build\contentserv" mkdir build\contentserv
if not exist "build\social" mkdir build\social

xcopy src\Server\PSOBB.Service.Authentication\bin\Debug_Local\netcoreapp2.0\publish build\auth /s /y
xcopy src\Server\PSOBB.Service.ServiceDiscovery\bin\Debug_Local\netcoreapp2.0\publish build\servdisc /s /y
xcopy src\Server\PSOBB.Service.ServerSelection\bin\Debug_Local\netcoreapp2.0\publish build\servsel /s /y

xcopy src\Server\PSOBB.Service.ServiceDiscovery\bin\Debug_Local\netcoreapp2.0\publish build\gameservdisc /s /y
xcopy src\Server\PSOBB.Service.GameServer\bin\Debug_Local\netcoreapp2.0\publish build\gameserv /s /y

xcopy src\Server\PSOBB.Service.ContentServer\bin\Debug_Local\netcoreapp2.0\publish build\contentserv /s /y

xcopy src\Server\PSOBB.Service.Social\bin\Debug_Local\netcoreapp2.1\publish build\Social /s /y

EXIT 0
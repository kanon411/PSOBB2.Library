dotnet publish src/Server/Guardians.Service.Authentication/Guardians.Service.Authentication.csproj -c DEBUG_LOCAL --self-contained -r win10-x64
dotnet publish src/Server/Guardians.Service.ServiceDiscovery/Guardians.Service.ServiceDiscovery.csproj -c DEBUG_LOCAL --self-contained -r win10-x64
dotnet publish src/Server/Guardians.Service.ServerSelection/Guardians.Service.ServerSelection.csproj -c DEBUG_LOCAL --self-contained -r win10-x64
dotnet publish src/Server/Guardians.Service.GameServer/Guardians.Service.GameServer.csproj -c DEBUG_LOCAL --self-contained -r win10-x64
dotnet publish src/Server/Guardians.Service.ContentServer/Guardians.Service.ContentServer.csproj -c DEBUG_LOCAL --self-contained -r win10-x64
dotnet publish src/Server/Guardians.Service.Social/Guardians.Service.Social.csproj -c DEBUG_LOCAL --self-contained -r win10-x64

if not exist "build\auth" mkdir build\auth
if not exist "build\servdisc" mkdir build\servdisc
if not exist "build\servsel" mkdir build\servsel
if not exist "build\gameservdisc" mkdir build\gameservdisc
if not exist "build\gameserv" mkdir build\gameserv
if not exist "build\contentserv" mkdir build\contentserv
if not exist "build\social" mkdir build\social

xcopy src\Server\Guardians.Service.Authentication\bin\Debug_Local\netcoreapp2.0\win10-x64\publish build\auth /s /y
xcopy src\Server\Guardians.Service.ServiceDiscovery\bin\Debug_Local\netcoreapp2.0\win10-x64\publish build\servdisc /s /y
xcopy src\Server\Guardians.Service.ServerSelection\bin\Debug_Local\netcoreapp2.0\win10-x64\publish build\servsel /s /y

xcopy src\Server\Guardians.Service.ServiceDiscovery\bin\Debug_Local\netcoreapp2.0\win10-x64\publish build\gameservdisc /s /y
xcopy src\Server\Guardians.Service.GameServer\bin\Debug_Local\netcoreapp2.0\win10-x64\publish build\gameserv /s /y

xcopy src\Server\Guardians.Service.ContentServer\bin\Debug_Local\netcoreapp2.0\win10-x64\publish build\contentserv /s /y

xcopy src\Server\Guardians.Service.Social\bin\Debug_Local\netcoreapp2.1\win10-x64\publish build\Social /s /y


EXIT 0
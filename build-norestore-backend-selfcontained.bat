dotnet publish src/Server/GladMMO.Service.Authentication/GladMMO.Service.Authentication.csproj -c DEBUG_LOCAL --self-contained -r win10-x64
dotnet publish src/Server/GladMMO.Service.ServiceDiscovery/GladMMO.Service.ServiceDiscovery.csproj -c DEBUG_LOCAL --self-contained -r win10-x64
dotnet publish src/Server/GladMMO.Service.ServerSelection/GladMMO.Service.ServerSelection.csproj -c DEBUG_LOCAL --self-contained -r win10-x64

if not exist "build\auth" mkdir build\auth
if not exist "build\servdisc" mkdir build\servdisc
if not exist "build\servsel" mkdir build\servsel

xcopy src\Server\GladMMO.Service.Authentication\bin\Debug_Local\netcoreapp2.0\win10-x64\publish build\auth /s /y
xcopy src\Server\GladMMO.Service.ServiceDiscovery\bin\Debug_Local\netcoreapp2.0\win10-x64\publish build\servdisc /s /y
xcopy src\Server\GladMMO.Service.ServerSelection\bin\Debug_Local\netcoreapp2.0\win10-x64\publish build\servsel /s /y


EXIT 0
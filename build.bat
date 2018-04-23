dotnet restore Guardians.Library.sln
dotnet publish src/Server/Guardians.Service.Authentication/Guardians.Service.Authentication.csproj -c release

if not exist "build" mkdir build
if not exist "build\auth" mkdir build\auth
xcopy src\Server\Guardians.Service.Authentication\bin\Release\netcoreapp1.1\publish build\auth /s /y

if not exist "build\auth\Certs" mkdir build\auth\Certs
if not exist "build\auth\Config" mkdir build\auth\Config

PAUSE
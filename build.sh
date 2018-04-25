dotnet restore Guardians.Library.sln
dotnet publish src/Server/Guardians.Service.Authentication/Guardians.Service.Authentication.csproj -c release

mkdir build
mkdir build/auth
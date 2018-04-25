dotnet restore Guardians.Library.sln
dotnet publish src/Server/Guardians.Service.Authentication/Guardians.Service.Authentication.csproj -c release

mkdir /app/build
mkdir /app/build/auth
mv /app/src/Server/Guardians.Service.Authentication/bin/Release/netcoreapp2.0/publish/* /app/build/auth
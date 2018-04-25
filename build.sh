dotnet restore Guardians.Library.sln
dotnet publish src/Server/Guardians.Service.Authentication/Guardians.Service.Authentication.csproj -c release

mkdir build
build\auth
mv src\Server\Guardians.Service.Authentication\bin\Release\netcoreapp2.0\publish build\auth
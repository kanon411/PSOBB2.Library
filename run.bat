cd build\auth
start "auth" cmd /c dotnet Guardians.Service.Authentication.dll --url=http://0.0.0.0:5001
cd ..
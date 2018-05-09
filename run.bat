cd build\auth
start "auth" cmd /c dotnet Guardians.Service.Authentication.dll --url=http://0.0.0.0:5001
cd ..

cd servdisc
start "servdisc" cmd /c dotnet Guardians.Service.ServiceDiscovery.dll --url=http://0.0.0.0:5000
cd ..

cd servsel
start "servsel" cmd /c dotnet Guardians.Service.ServerSelection.dll --url=http://0.0.0.0:5002
cd ..

cd gameservdisc
start "gameservdisc" cmd /c dotnet Guardians.Service.ServiceDiscovery.dll --url=http://0.0.0.0:5003
cd ..

cd gameserv
start "gameserv" cmd /c dotnet Guardians.Service.GameServer.dll --url=http://0.0.0.0:5004
cd ..
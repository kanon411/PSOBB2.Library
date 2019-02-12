cd build\auth
start "auth" cmd /c dotnet PSOBB.Service.Authentication.dll --url=http://192.168.0.3:5001
cd ..

cd servdisc
start "servdisc" cmd /c dotnet PSOBB.Service.ServiceDiscovery.dll --url=http://192.168.0.3:5000
cd ..

cd servsel
start "servsel" cmd /c dotnet PSOBB.Service.ServerSelection.dll --url=http://192.168.0.3:5002
cd ..

cd gameservdisc
start "gameservdisc" cmd /c dotnet PSOBB.Service.ServiceDiscovery.dll --url=http://192.168.0.3:5003
cd ..

cd gameserv
start "gameserv" cmd /c dotnet PSOBB.Service.GameServer.dll --url=http://192.168.0.3:5004
cd ..

cd contentserv
start "contentserv" cmd /c dotnet PSOBB.Service.ContentServer.dll --url=http://192.168.0.3:5005
cd ..

cd social
start "social" cmd /c dotnet PSOBB.Service.Social.dll --url=http://192.168.0.3:5008
cd ..
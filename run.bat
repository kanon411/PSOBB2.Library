cd build\auth
start "auth" cmd /c dotnet Guardians.Service.Authentication.dll --usehttps=Certs/TLSCert.pfx --url=http://localhost:5001
cd ..
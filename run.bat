cd build\auth
start "auth" cmd /c dotnet Guardians.Service.Authentication.dll --usehttps=Certs/TLSCert.pfx --url=http://127.0.0.1:5001
cd ..
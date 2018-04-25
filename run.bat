cd build\auth
start "auth" cmd /c dotnet Guardians.Service.Authentication.dll --usehttps=Certs/TLSCert.pfx --url=http://172.16.42.120:5001
cd ..
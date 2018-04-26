cd auth
start "auth" cmd /c dotnet Guardians.Service.Authentication.dll --usehttps=Certs/TLSCert.pfx --url=http://0.0.0.0:5001
cd ..
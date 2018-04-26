if not exist "build" mkdir build
if not exist "build\auth" mkdir build\auth
xcopy src\Server\Guardians.Service.Authentication\bin\Release\netcoreapp2.0 build\auth /s /y

xcopy run-aws-full.bat build /s /y

PAUSE
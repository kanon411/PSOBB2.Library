dotnet publish src/Guardians.Client.All/Guardians.Client.All.csproj -c Debug

if not exist "build\client" mkdir build\client

xcopy src\Guardians.Client.All\bin\Debug\netstandard2.0\publish build\client /Y /q /EXCLUDE:BuildExclude.txt
EXIT 0
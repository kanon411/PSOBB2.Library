dotnet publish src/PSOBB.Client.All/PSOBB.Client.All.csproj -c Debug

if not exist "build\client" mkdir build\client

xcopy src\PSOBB.Client.All\bin\Debug\netstandard2.0\publish build\client /Y /q /EXCLUDE:BuildExclude.txt
EXIT 0
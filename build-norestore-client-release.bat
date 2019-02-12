dotnet publish src/PSOBB.Client.All/PSOBB.Client.All.csproj -c Release

if not exist "build\client" mkdir build\client

xcopy src\PSOBB.Client.All\bin\Release\netstandard2.0\publish build\client /Y /q /EXCLUDE:BuildExclude.txt
EXIT 0
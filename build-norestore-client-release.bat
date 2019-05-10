dotnet publish src/GladMMO.Client.All/GladMMO.Client.All.csproj -c Release

if not exist "build\client" mkdir build\client

xcopy src\GladMMO.Client.All\bin\Release\netstandard2.0\publish build\client /Y /q /EXCLUDE:BuildExclude.txt
EXIT 0
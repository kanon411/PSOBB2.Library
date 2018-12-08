dotnet publish src/Dissonance/Dissonance.GladNet.Client/Dissonance.GladNet.Client.csproj -c Debug
dotnet publish src/Dissonance/Dissonance.Editor/Dissonance.Editor.csproj -c Debug

if not exist "build\voice" mkdir build\voice

xcopy src\Dissonance\Dissonance.GladNet.Client\bin\Debug\netstandard2.0\publish build\voice /Y /q /EXCLUDE:BuildExclude.txt
xcopy src\Dissonance\Dissonance.GladNet.Editor\bin\Debug\netstandard2.0\publish build\voice /Y /q /EXCLUDE:BuildExclude.txt
EXIT 0
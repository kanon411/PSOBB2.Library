call build-norestore-client.bat

xcopy build\client  C:\Users\Glader\Documents\GitHub\PSOBB2.Client\Assets\DLLs /Y /q /EXCLUDE:BuildExclude.txt
PAUSE
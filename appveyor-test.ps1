##Looks through the entire test directory and runs dotnet build
##foreach file in test/*
foreach($f in Get-ChildItem ./test/)
{
    ##foreach file in the src/*/ directory that ends with the .csproj format
    foreach($ff in (Get-ChildItem (Join-Path ./test/ $f.Name) | Where-Object { $_.Name.EndsWith(".csproj") }))
    {
        ##Add the project path + the csproj name and add the include referenced projects argument which will
        ##force nuget dependencies
        $projectArgs = "test " + (Join-Path (Join-Path test/ $f.Name) $ff.Name)## + ""
        Start-Process dotnet $projectArgs -Wait
    }
}

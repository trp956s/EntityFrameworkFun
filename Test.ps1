dotnet --info;
if($?) { 
    dotnet test WebApplication1.Test --logger "console;verbosity=normal"
}

if($?) { 
    dotnet test WebApplication1.Data.Test --logger "console;verbosity=normal"
} else {
    dotnet test WebApplication1.Data.Test --logger "console;verbosity=normal"
    throw "WebApplication1.Test failed"
}


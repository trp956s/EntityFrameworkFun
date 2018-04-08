Uninstall-Package FakeItEasy  
Uninstall-Package Microsoft.EntityFrameworkCore
Uninstall-Package Microsoft.EntityFrameworkCore.InMemory 

Install-Package FakeItEasy -ProjectName "WebApplication1.Data.Test"
Install-Package Microsoft.EntityFrameworkCore -Version 2.0.2 -ProjectName "WebApplication1.Data.Test"
Install-Package Microsoft.EntityFrameworkCore.InMemory -Version 2.0.2 -ProjectName "WebApplication1.Data.Test"
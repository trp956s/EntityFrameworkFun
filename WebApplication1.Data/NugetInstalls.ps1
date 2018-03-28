Uninstall-Package Microsoft.EntityFrameworkCore
Uninstall-Package Microsoft.EntityFrameworkCore.SqlServer 
Uninstall-Package Microsoft.EntityFrameworkCore.Tools 
Uninstall-Package Microsoft.VisualStudio.Web.CodeGeneration.Design 
Uninstall-Package Microsoft.EntityFrameworkCore.Design
Uninstall-Package EntityFramework 

Install-Package Microsoft.EntityFrameworkCore -Version 2.0.2 -ProjectName "WebApplication1.Data"
Install-Package Microsoft.EntityFrameworkCore.SqlServer  -Version 2.0.2 -ProjectName "WebApplication1.Data"
Install-Package Microsoft.EntityFrameworkCore.Tools -Version 2.0.2 -ProjectName "WebApplication1.Data"
Install-Package Microsoft.VisualStudio.Web.CodeGeneration.Design -ProjectName "WebApplication1.Data"
Install-Package Microsoft.EntityFrameworkCore.Design -Version 2.0.2 -ProjectName "WebApplication1.Data"
Install-Package EntityFramework -ProjectName "WebApplication1.Data"

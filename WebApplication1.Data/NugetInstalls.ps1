
Uninstall-Package Microsoft.EntityFrameworkCore
Uninstall-Package Microsoft.EntityFrameworkCore.SqlServer 
Uninstall-Package Microsoft.EntityFrameworkCore.Tools 
Uninstall-Package Microsoft.VisualStudio.Web.CodeGeneration.Design 
Uninstall-Package Microsoft.EntityFrameworkCore.Design
Uninstall-Package EntityFramework 

Install-Package Microsoft.EntityFrameworkCore -ProjectName "WebApplication1.Data"
Install-Package Microsoft.EntityFrameworkCore.SqlServer -ProjectName "WebApplication1.Data"
Install-Package Microsoft.EntityFrameworkCore.Tools -ProjectName "WebApplication1.Data"
Install-Package Microsoft.VisualStudio.Web.CodeGeneration.Design -ProjectName "WebApplication1.Data"
Install-Package Microsoft.EntityFrameworkCore.Design -ProjectName "WebApplication1.Data"
Install-Package EntityFramework -ProjectName "WebApplication1.Data"


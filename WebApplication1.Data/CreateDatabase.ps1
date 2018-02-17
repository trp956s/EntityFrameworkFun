EntityFrameworkCore\Enable-Migrations -EnableAutomaticMigrations -ContextProjectName "WebApplication1.Data" -StartupProjectName WebApplication1 -ConnectionStringName DefaultConnection

EntityFrameworkCore\Update-Database -Migration:0
EntityFrameworkCore\Remove-Migration
EntityFrameworkCore\Add-Migration InitialCreate
EntityFrameworkCore\Update-Database
Ssms.exe -S "(localdb)\MSSQLLocalDB"
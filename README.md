# MOG_Solution 

### Repoditory
https://github.com/keiloa-ise/MOG_APIs

### Prerequisites
- Modern API Design with CQRS
- .NET 9.0 SDK or newer
- SQL Server 

### Installation
1. Clone the repo
2. Update connection strings in `appsettings.json`
3. Migrations (Package Manager Console):
    * cd APIs
    * dotnet restore
    * (SKIP) sqlcmd -S "(localdb)\MSSQLLocalDB" -d "MOJ_Users" -Q "DROP TABLE IF EXISTS __EFMigrationsHistory"
    * (SKIP) Remove-Migration
    * dotnet build
    * Add-Migration InitialCreateWithRoles -OutputDir Data\Migrations
    * Update-Database

## Content
| Project  | Discription | Type |
| ------------- |:-------------:|:-------------:|
| APIs | Main Project | Web API |
| UserManagments | User Managment Logic | Class Library |
| Shared | Shared | Class Library |
# MOG_Solution 

### Repoditory
https://github.com/keiloa-ise/MOG_APIs

### Prerequisites
- .NET 9.0 SDK or newer
- SQL Server 

### Installation
1. Clone the repo
2. Update connection strings in `appsettings.json`
3. Migrations (Package Manager Console):
    * cd APIs
    * dotnet restore
    * dotnet build
    * Add-Migration InitialCreate -OutputDir "Infrastructure/Persistence/Migrations"
    * Update-Database

## Content
| Project  | Discription | Type |
| ------------- |:-------------:|:-------------:|
| APIs | Main Project | Web API |
| UserManagments | User Managment Logic | Class Library |


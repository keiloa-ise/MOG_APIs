# MOG_Solution Repo
https://github.com/keiloa-ise/MOG_APIs

### Prerequisites
- .NET 9.0 SDK or newer
- SQL Server 

### Installation
1. Clone the repo
2. Update connection strings in `appsettings.json`
3. Migrations:
   Package Manager Console:
	cd APIs
    # dotnet
    dotnet restore
    dotnet build
	# Migration
	Add-Migration InitialCreate -OutputDir "Infrastructure/Persistence/Migrations"
	Update-Database
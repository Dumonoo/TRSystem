# Time Reporting System - ASP .NET

## Requirements
* MySQL Server 5.7.37
* .NET SDK 5.0.402

## Preparation
1. Crete database on localhost
    * Port: 3306
    * databaseName: TRSystem
    * UserName:  NTR21
    * Password: petclinic
2. Create migration and update database schema
```bash
    dotnet ef migrations add InitialCreate
    dotnet ef database update
```
3. Run project
```bash
    dotnet run
```
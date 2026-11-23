# School Management System

A School Management System built with ASP.NET Core MVC and Entity Framework Core using the Code First approach with MySQL as the database.

In Code First, you define the database structure with C# model classes first. Entity Framework Core then creates the database tables from those classes using migrations. You do not create the tables manually in MySQL.

## Tech Stack

| Technology | Version / Detail |
| --- | --- |
| Framework | ASP.NET Core MVC |
| Target Framework | .NET 10.0 |
| ORM | Entity Framework Core 10.0.15 |
| MySQL Provider | Pomelo.EntityFrameworkCore.MySql 8.0.3 |
| Database | MySQL Server 8.0 |
| IDE | Visual Studio 2022 |

## Prerequisites

Before you start, install the following:

- Visual Studio 2022 with the ASP.NET and web development workload
- .NET 10 SDK
- MySQL Server 8.0 (through MySQL Installer or XAMPP)
- MySQL Workbench (to inspect the database)
- Git

## Setup

### Step 1. Clone the Repository

```bash
git clone https://github.com/SohaibKhaliq/SchoolManagement-ASP.NET-MVC.git
cd SchoolManagement-ASP.NET-MVC
```

### Step 2. Open the Project in Visual Studio

1. Open `SchoolManagement.slnx` in Visual Studio 2022.
2. Restore the NuGet packages. Visual Studio restores them automatically on build. You can also right-click the solution in Solution Explorer and select Restore NuGet Packages.

The required Entity Framework Core packages are already referenced in `SchoolManagement.csproj`:

| Package | Version | Purpose |
| --- | --- | --- |
| Pomelo.EntityFrameworkCore.MySql | 8.0.3 | MySQL provider for EF Core |
| Microsoft.EntityFrameworkCore.Design | 10.0.15 | Design-time support for migrations |
| Microsoft.EntityFrameworkCore.Tools | 10.0.15 | Package Manager Console commands such as Add-Migration |

### Step 3. Start MySQL and Create the Database

1. Make sure MySQL Server is running.
2. Open MySQL Workbench and connect to your local server.
3. Create the database used by this project:

```sql
CREATE DATABASE school CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

If you use a different database name, update the connection string in Step 4 to match it.

### Step 4. Update the Connection String

Open `appsettings.json`. It already contains a connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=school;uid=root;pwd=;"
}
```

Set `pwd` to your MySQL root password. If MySQL runs on a different port, add `Port=3306;` (or your port number) to the connection string. Do not commit a real production password to the repository.

### Step 5. Create the Models

Create a model class for each table in the `Models` folder. This is the Code First part: the class defines the table, and each property defines a column.

Example: create `Models/Student.cs`:

```csharp
namespace SchoolManagement.Models;

public class Student
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
```

Repeat this step for every other entity the system needs, for example Teacher, Class, and Enrollment.

### Step 6. Create the DbContext

Create a `Data` folder and add `Data/SchoolDbContext.cs`. The DbContext is the class that connects your models to the database. Each `DbSet` property represents one table.

```csharp
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Models;

namespace SchoolManagement.Data;

public class SchoolDbContext : DbContext
{
    public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }
}
```

Add one `DbSet` property for each model class you created in Step 5.

### Step 7. Register the DbContext in Program.cs

Open `Program.cs` and register the DbContext before the line `var app = builder.Build();`:

```csharp
using Microsoft.EntityFrameworkCore;
using SchoolManagement.Data;

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseMySql(connectionString,
        ServerVersion.AutoDetect(connectionString)));
```

`ServerVersion.AutoDetect` reads the MySQL server version from the running server. If you prefer to set it explicitly, replace it with your server version, for example:

```csharp
new MySqlServerVersion(new Version(8, 0, 36))
```

You can find your server version in MySQL Workbench with `SELECT VERSION();`.

### Step 8. Add the First Migration

A migration is a C# file that describes the database changes needed to match your models. It is created with the Package Manager Console.

1. In Visual Studio, go to Tools, then NuGet Package Manager, then Package Manager Console.
2. Make sure the Default Project is set to `SchoolManagement`.
3. Run:

```powershell
Add-Migration InitialCreate
```

This creates a `Migrations` folder in the project. If you prefer the .NET CLI instead, run:

```bash
dotnet ef migrations add InitialCreate
```

For the CLI command, first install the EF Core tools globally with `dotnet tool install --global dotnet-ef`.

### Step 9. Create the Database Tables

Run the migration against the database to create the tables:

```powershell
Update-Database
```

Or with the .NET CLI:

```bash
dotnet ef database update
```

Verify the result in MySQL Workbench:

```sql
USE school;
SHOW TABLES;
```

You should see a `Students` table and a `__EFMigrationsHistory` table. The history table records which migrations have already been applied.

### Step 10. Run the Project

Press `F5` in Visual Studio, or run from the terminal:

```bash
dotnet run
```

The application runs at:

- HTTPS: `https://localhost:7032`
- HTTP: `http://localhost:5096`

## Changing the Database Later

When you add a model or change a model class, do not edit the database tables directly. Follow the migration workflow:

1. Change the model class in the `Models` folder.
2. Add a new migration with a descriptive name:

```powershell
Add-Migration AddTeacherTable
```

3. Apply it to the database:

```powershell
Update-Database
```

Each change gets its own migration. This keeps the code and the database in sync and lets other developers apply the same changes by running `Update-Database`.

## Project Structure

```text
SchoolManagement-ASP.NET-MVC/
├── Controllers/             # MVC controllers
├── Data/                    # SchoolDbContext (created in Step 6)
├── Migrations/              # Generated migration files (created in Step 8)
├── Models/                  # Entity classes, for example Student
├── Views/                   # Razor views
├── wwwroot/                 # Static files (CSS, JS, libraries)
├── appsettings.json         # Connection string
├── Program.cs               # Application startup and DbContext registration
├── SchoolManagement.csproj  # Project file and NuGet package references
└── SchoolManagement.slnx    # Visual Studio solution file
```

## Troubleshooting

| Problem | Solution |
| --- | --- |
| `Add-Migration` is not recognized | Make sure `Microsoft.EntityFrameworkCore.Tools` is installed, the Default Project in Package Manager Console is `SchoolManagement`, and the project builds without errors. |
| Unable to connect to MySQL | Check that MySQL Server is running, and that the server, port, user, and password in `appsettings.json` are correct. |
| Unknown database `school` | Create the database as shown in Step 3, or run `Update-Database` and let EF Core create it. |
| No tables are created after `Update-Database` | Check that the DbContext is registered in `Program.cs` and that the model classes are added as `DbSet` properties in `SchoolDbContext`. |
| Package version conflict on restore | This project targets .NET 10 and EF Core 10.0.15, while `Pomelo.EntityFrameworkCore.MySql` in the project file is version 8.0.3, which targets EF Core 8. If restore or `Add-Migration` fails with a version conflict, update the MySQL provider to a version that matches EF Core 10 before running the first migration. |

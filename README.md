# In Memory Database Testing POC

## Summary
The purpose of this POC (Proof of Concept), is to document the differences between Microsoft and SQLite for in memory unit testing a data access layer that implements Entity Framework and a Microsoft SQL database. I have implemented version 5.0.5 of the frameworks listed below:
- [Microsoft.EntityFrameworkCore.InMemory](https://docs.microsoft.com/en-us/ef/core/providers/in-memory/?tabs=dotnet-core-cli)
- [Microsoft.Data.Sqlite.Core](https://docs.microsoft.com/en-us/dotnet/standard/data/sqlite/?tabs=netcore-cli) & [Microsoft.EntityFrameworkCore.Sqlite](https://docs.microsoft.com/en-us/ef/core/providers/sqlite/?tabs=dotnet-core-cli)

Unit testing a data access layer utilizing .NET Core’s Entity Framework can be a challenge. The two popular choices are Microsoft and Sqlite. Both have their pros and cons but what they have in common is both do not accurately simulate the real thing. Once you have come to grips with this concept, you will have to strategize how can my team successfully utilize either framework and still provide effective test coverage over your data access layer. Based on the following results, I’m going to favour Sqlite, but there are some unexpected side effects.
## Technology Stack
- MS SQL relational database 2019^
- Entity Framework 5.0.5
- Code First Migrations 5.0.5 

[Testing code that uses EF Core](https://docs.microsoft.com/en-us/ef/core/testing/)

## Pros & Cons
| Use Case | MS | SQLite
| --- | --- | ---
| Primary Key Identity, auto increment |  Supported | I'm probably doing something wrong however, I was unable to get Sqlite to work with primary keys of type int, configured to identity. I attempted to configure in the Fluent API with .ValueGeneratedOnAdd() but with no success.  If you are using Guid's with an sql default newid() there is a .CreateFunction() helper.
| Can test invalid property values | Whilst this is not supported you could / should test via [model state validation](https://docs.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-5.0) in the controllers before the data progresses to the data access layer. | Supported
| Can test unique index constraint | | Supported, throws a DbUpdateException however, if you want to get the SQL error numbers to differentiate between unique constraint violation or something other, you need to cast via SqliteException instead of SqlException.
| Concurrency | Supported | Supported
| Migrations | Supported | In older versions of .NET Core 3.0.1 LTS, I found on occasions where a migration generated an incompatible SQL command with Sqlite,  I found I had to wrap migrations in conditionals ```if(migrationBuilder.IsSqlServer()) { do normal migration } else { drop table, then create }``` In this POC, I’m using .NET Core 5.0.5 and was unable to reproduce. Kudos to the team managing this framework.
| Seed / Migrate Data | Supported | I did not test seeding in this POC. Whilst I prefer not to seed or migrate data within migrations, sometimes you have to do what you have to do. It is recommended to implement the [MigrationBuilder](https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.migrations.migrationbuilder?view=efcore-5.0) methods (.InsertData(), .UpdateData(), …) as opposed to using ```migrationBuilder.Sql(native TSQl commands)``` and let the framework attempt to translate between native MS TSQL & Sqlite
| Other discovered incompatibilities | | In version 5.0.5 there is no support for nvarchar(max). Recently discovered by a work colleague just as I published this repo. Luckily, they also discovered a work around. However now I can't test for properties exceeding 4000 characters. 

### What I have discovered so far
Based on the Pros & Cons table, Sqlite looks very attractive however, there are hidden side effects. Whilst unique constraints can be tested for and a DbUpdateException is thrown, the sub exception differs and so does the error number: 
- MS SqlException: 2601 
- Sqlite SqliteException:  19

NVARCHAR(MAX):
SQLite does not know what nvarchar(max) means and generates the following error: 
```
Microsoft.Data.Sqlite.SqliteException : SQLite Error 1: 'near "max": syntax error'.
```
If you set a string length constraint greater then 4000, Code First will add:
```
migrationBuilder.AddColumn<string>(
    name: "Data",
    table: "Products",
    type: "nvarchar(max)", //  MAX even if you specify 4001
    nullable: true);
```
.NET Core provides us with the [DbCommandInterceptor](https://docs.microsoft.com/en-us/dotnet/api/system.data.entity.infrastructure.interception.dbcommandinterceptor?view=entity-framework-6.2.0&viewFallbackFrom=entity-framework-5.0.5) and this has been implemented to overcome but not entirely solve this current issue. Keep in mind, we are opting to use SQLite to provide a more granular testing environment. Now, if I want to test for entity property/s that exceed string lengths of 4000 characters, I cannot. However, another strategy would be to test with model state validation when testing endpoints.

Other side effects include:
- In older versions of .NET core such as 3.0.1 LTS, migrations have to be wrapped in conditionals
- Seed / Migration data, its strategically safer to run commands in specific migrationBuilder extension methods as opposed to running native TSQL migrationBuilder.Sql().

## Conclusion
Utilizing Sqlite for in memory unit testing seems to have a couple of benefits over the Microsoft alternative and as the framework continues to mature, some of the side effects I’ve encountered in the past are now resolved. Based on the above findings and a years’ worth of experience working with Sqlite with .NET Core (3.0.1 to 5.0.4), I would highly recommend using it in current and future projects.

**Caveat:** I was unable to resolve my issues with integer primary keys. I’m sure it was just a configuration issue I was unable to resolve in the time I set to do this POC.

# References:
- [Microsoft.EntityFrameworkCore.InMemory -v5.0.5](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.InMemory/5.0.5)
- [Microsoft.Data.Sqlite.Core -v5.0.5](https://www.nuget.org/packages/Microsoft.Data.Sqlite.Core/5.0.5)
- [Microsoft.EntityFrameworkCore.Sqlite -v5.0.5](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite/5.0.5)
- [Database engine errors](https://docs.microsoft.com/en-us/sql/relational-databases/errors-events/database-engine-events-and-errors?view=sql-server-ver15) There are many.

## Issues Encounted

### Primary Keys of type int, configured to identity

Implementing primary keys as type int and configured to identity worked initially although I did get this error:
```
---- Microsoft.Data.Sqlite.SqliteException : SQLite Error 19: 'NOT NULL constraint failed: Products.Id'.
``` 
To resolve this I assigned values to primary keys to test data and it worked however I would have thought the translation form SQL identity to Sqlite autoincrement should have taken care of this.

The next showstopper happened when I introduced a foreign key constraint between Product & ProductJunture. Then I got this error:
```
Microsoft.Data.Sqlite.SqliteException : SQLite Error 1: 'AUTOINCREMENT is only allowed on an INTEGER PRIMARY KEY'.
```
This is where I stopped and migrated to implementing Guid's as primary & foreign keys.  If you work this out, please don't hesitate to let me know.
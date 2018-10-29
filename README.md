# AP

AP is RESTful API for creating simple blogs. This project has been created for my own blog [Artysta Programista](https://artystaprogramista.pl).

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites

To make this project build and run you need to download and install [.NET Core](https://www.microsoft.com/net/download) with version 2.1 or higher.

(Recommended) This project works well with [Docker](https://www.docker.com/) and I personally recommend to install docker on your machine and run this project with several lines of commands. 

(Optional) If you are interested in runnning this project even faster you can use our Makefile with [Make](https://www.gnu.org/software/make/)

### Installing

A step by step series of examples that tell you how to get a development env running. If you want to use docker check out how to run it with docker on our Wiki.

Clone repository 

```
git clone https://github.com/Kubion70/AP.git
```

When you have a folder with copy of repository open the DatabaseContext.cs placed in  ./AP.Repositories/Context . Change the connection string to let server connect to MSSQL database. (If your are using different RDBMS you can change the method `UseSqlServer("")` based on [Entity Framework supported databases](https://entityframework.net/supported-database-providers))

```
optionsBuilder.UseSqlServer("Data Source=IP_ADDRESS;Initial Catalog=DATABASE_NAME;User ID=USERNAME;Password=PASSWORD;");
```

Open terminal and go to the main folder. Run command below to restore projects dependencies and tools.

```
dotnet restore
```

Use migrations to create actual database structure

```
dotnet ef migrations add $(date '+%d%m%Y%H') --startup-project AP.Repositories --project AP.Repositories
```

Update database with generated migrations

```
dotnet ef database update --startup-project AP.Repositories --project AP.Repositories
```

To run project (development mode) use this command

```
dotnet run --project AP.Web
```

When the application will be up and running go to https://localhost:5001/api where  [Swagger](https://swagger.io/) will show you all endpoints.

## Deployment

For production mode replace the `dotnet run` command with

```
dotnet publish AP.Web -c Release
```

and to run it use

```
dotnet AP.Web.dll
```

## Built With

* [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/getting-started/?view=aspnetcore-2.1&tabs=linux)
* [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) - ORM
* [AutoMapper](http://docs.automapper.org/en/stable/index.html) - Entity Mapping

## Authors

* **Jakub Kowalski** - *Main work* - [Kubion70](https://github.com/Kubion70)

See also the list of [contributors](https://github.com/Kubion70/AP/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.


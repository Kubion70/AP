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

```bash
git clone https://github.com/Kubion70/AP.git
```

When you have a copy of repository it's time to setup JwT secret key (needed for your users authorization). Go AP<span>.</span>Web folder, open `appsettings.json` and replace the `secret key` string with your own (it is a good practice to user at least SHA256 generated key).

Open terminal and go to the main folder. Run command below to restore projects dependencies and tools.

```bash
dotnet restore
```

To run project (development mode) use this command

```bash
dotnet run -p AP.Web
```

When the application will be up and running go to <https://localhost:5001/api> where  [Swagger](https://swagger.io/) will show you all endpoints.

**Remember** that development mode runs with database in memory. What means all stored data clears when the program stops.

## Running the tests

Our tests are mainly focues on endpoints test (end to end as well). To run it go to the main folder and run following command:

```bash
dotnet test
```

If you have configured the project correctly all tests should bee passed successfully.

## Deployment

Open the DatabaseContext.cs placed in  ./AP.Repositories/Context . Change the connection string to let server connect to MSSQL database. (If your are using different RDBMS you can change the method `UseSqlServer("")` based on [Entity Framework supported databases](https://entityframework.net/supported-database-providers))

```csharp
optionsBuilder.UseSqlServer("Data Source=IP_ADDRESS;Initial Catalog=DATABASE_NAME;User ID=USERNAME;Password=PASSWORD;");
```

For production mode publish app with

```bash
dotnet publish AP.Web -c Release
```

and to run it use

```bash
ASPNETCORE_ENVIRONMENT=Production dotnet AP.Web.dll
```

## Built With

* [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/getting-started/?view=aspnetcore-2.1&tabs=linux)
* [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) - ORM
* [AutoMapper](http://docs.automapper.org/en/stable/index.html) - Entity Mapping
* [CacheManager](http://cachemanager.michaco.net/) - Cache
* [XUnit](https://xunit.github.io/) - Unit Testing

## Authors

* **Jakub Kowalski** - *Main work* - [Kubion70](https://github.com/Kubion70)

See also the list of [contributors](https://github.com/Kubion70/AP/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.
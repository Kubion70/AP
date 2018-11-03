using Microsoft.EntityFrameworkCore;
using System;
using Models = AP.Entities.Models;

namespace AP.Repositories.Contexts
{
    public class DatabaseContext : DbContext
    {
        #region Entities

        public DbSet<Models.User> Users { get; set; }
        public DbSet<Models.Category> Categories { get; set; }
        public DbSet<Models.Post> Posts { get; set; }

        #endregion Entities

        #region Ctor

        public DatabaseContext()
        {
        }

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        #endregion Ctor

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            switch(env)
            {
                case "Production":
                    optionsBuilder.UseSqlServer("Data Source=IP_ADDRESS;Initial Catalog=DATABASE_NAME;User ID=USERNAME;Password=PASSWORD;");
                    break;
                default: case "Development":
                    optionsBuilder.UseInMemoryDatabase(databaseName: "TestDB");
                    break;
            }

            base.OnConfiguring(optionsBuilder);
        }
    }
}
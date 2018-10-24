using Microsoft.EntityFrameworkCore;
using System;
using Models = AP.Entities.Models;

namespace AP.Repositories.Contexts
{
    public class DatabaseContext : DbContext
    {
        #region Entities

        public DbSet<Models.SystemUser> SystemUsers { get; set; }
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
            optionsBuilder.UseSqlServer("Data Source=xxx.xx.x.x;Initial Catalog=DbName;User ID=sa;Password=strongPassword123;");

            base.OnConfiguring(optionsBuilder);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using AP.Cryptography;
using AP.Repositories.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models = AP.Entities.Models;

namespace AP.Web.Extensions
{
    public static class ApplicationExtensions
    {
        public static IApplicationBuilder UpdateDatabase(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<DatabaseContext>())
                {
                    if (!context.Database.IsInMemory())
                    {
                        context.Database.Migrate();
                    }
                    else
                    {
                        var isAnyUserCreated = context.Users.AnyAsync().Result;

                        if (!isAnyUserCreated)
                        {
                            var admin = new Models.User(Guid.NewGuid())
                            {
                                Username = "Admin",
                                Password = SHA.GenerateSHA256String("admin"),
                                Email = "admin@admin.ad",
                                FirstName = "John",
                                LastName = "Doe"
                            };

                            context.Users.Add(admin);
                            context.SaveChanges();
                        }
                    }
                }
            }

            return app;
        }
    }
}
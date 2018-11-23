using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using AP.Web.Extensions;
using System.Reflection;
using System.IO;
using AutoMapper;
using AP.Entities.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using AP.Repositories.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AP.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        private IConfiguration Configuration { get; }

        private IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            
            services.AddDbContext<DatabaseContext>(optionsBuilder =>
            {
                if (HostingEnvironment.IsProduction())
                    optionsBuilder.UseSqlServer(Configuration.GetConnectionString("Default"));
                else
                {
                    optionsBuilder.UseInMemoryDatabase(databaseName: "TestDB");
                    // Transactions are not supported with in memory database
                    optionsBuilder.ConfigureWarnings(p => p.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                }
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                });

            // Register all dependencies
            services.RegisterServices();

            services.AddMvc(setup => 
            {
                var authPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                setup.Filters.Add(new AuthorizeFilter(authPolicy));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddAutoMapper(x => x.AddProfile(new MappingEntity()));

            if (HostingEnvironment.IsDevelopment() || !string.IsNullOrEmpty(Configuration.GetConnectionString("Default")))
            {
                // Register the Swagger generator, defining 1 or more Swagger documents
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = "AP", Version = "v1" });

                    c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        In = "header",
                        Type = "apiKey"
                    });

                    c.OperationFilter<SecurityRequirementsOperationFilter>();

                    // Set the comments path for the Swagger JSON and UI.
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
                });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCors(builder => 
                builder.WithOrigins("https://localhost:5001", "http://localhost:3000")
                .WithExposedHeaders("X-Total-Count")
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UpdateDatabase();
            if (env.IsDevelopment())
            {
                if (HostingEnvironment.IsDevelopment() || !string.IsNullOrEmpty(Configuration.GetConnectionString("Default")))
                {
                    app.UseStaticFiles();
                    // Enable middleware to serve generated Swagger as a JSON endpoint.
                    app.UseSwagger();

                    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
                    // specifying the Swagger JSON endpoint.
                    app.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AP V1");
                        c.RoutePrefix = "api";
                        c.InjectStylesheet("../apiStyles/theme-material.css");
                    });
                }

                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc(routes =>
            {
                if (HostingEnvironment.IsProduction() && string.IsNullOrEmpty(Configuration.GetConnectionString("Default")))
                {
                    routes.MapRoute("configuration", "/api",
                        defaults: new { controller = "Home", action = "Index" });
                }
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AP.Cryptography;
using AP.Entities.Models;
using AP.Repositories.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AP.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        private readonly IApplicationLifetime _applicationLifetime;

        public HomeController(IConfiguration configuration, IApplicationLifetime applicationLifetime)
        {
            _configuration = configuration;
            _applicationLifetime = applicationLifetime;
        }
        
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            var connectionString = _configuration.GetConnectionString("Default");
            if (connectionString == null)
                return View(new ConfigModel());
            else
                return View("Configured");
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Index(ConfigModel configModel)
        {
            if (!ModelState.IsValid)
                return View(configModel);

            try
            {
                var options = new DbContextOptionsBuilder()
                    .UseSqlServer(configModel.ConnectionString)
                    .Options;

                using (var databaseContext = new DatabaseContext(options))
                {
                    databaseContext.Database.OpenConnection();

                    databaseContext.Database.Migrate();

                    databaseContext.Users.Add(new User()
                    {
                        Username = configModel.Username,
                        Password = SHA.GenerateSHA256String(configModel.Password),
                        Email = configModel.Email
                    });
                    databaseContext.SaveChanges();

                    databaseContext.Database.CloseConnection();
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("ConnectionString", "Could not connect to database!");
                return View(configModel);
                throw ex;
            }

            var _settings = new 
            {
                Logging = new 
                {
                    LogLevel = new
                    {
                        Default = "Warning"
                    }
                },
                ConnectionStrings = new
                {
                    Default = configModel.ConnectionString
                },
                Jwt = new
                {
                    Key = SHA.GenerateSHA256String(Guid.NewGuid().ToString()),
                    Issuer = $"{this.Request.Scheme}://{this.Request.Host}",
                    TokenExpire = 30
                }
            };
            string json = JsonConvert.SerializeObject(_settings, Formatting.Indented);
            System.IO.File.WriteAllText("appsettings.json", json);

            return View("Configured");
        }
    }

    public class ConfigModel
    {
        [Required]
        public string ConnectionString { get; set; } = "";

        [Required]
        [MinLength(5)]
        public string Username { get; set; } = "";

        [Required]
        [RegularExpression(@"^(?=.*[A-Z].*[A-Z])(?=.*[!@#$&*])(?=.*[0-9].*[0-9])(?=.*[a-z].*[a-z].*[a-z]).{8,}$", ErrorMessage = "Password is unsecure!")]
        public string Password { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
    }
}
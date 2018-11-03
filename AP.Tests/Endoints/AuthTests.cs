using AP.Web.Controllers;
using AP.Repositories.User;
using Xunit;
using Models = AP.Entities.Models;
using CacheManager.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System;
using System.Threading.Tasks;
using AP.Cryptography;
using System.Security.Cryptography;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

namespace AP.Tests.Endpoints
{
    public class AuthTests : IClassFixture<WebApplicationFactory<AP.Web.Startup>>
    {
        private readonly WebApplicationFactory<AP.Web.Startup> _factory;

        private readonly IUserRepository _userRepository;

        // Preparings for tests
        public AuthTests(WebApplicationFactory<AP.Web.Startup> factory)
        {
            _factory = factory;

            _userRepository = new UserRepository();
            
            var adminUser = new Models.User()
            {
                Username = "Admin",
                Password = SHA.GenerateSHA256String("AAdmin12!"),
                Email = "admin@admin.pl",
                FirstName = "Adam",
                LastName = "Adamowski"
            };

            _userRepository.Create(adminUser).Wait();
        }

        [Theory]
        [InlineData("Admin", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public async Task IsSaltGeneratingCorrectly(string username, bool sucessfulResponse)
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"api/auth/{username}");
            string responseBody = await response.Content.ReadAsStringAsync();
            Assert.Equal(sucessfulResponse, response.IsSuccessStatusCode);

            if(sucessfulResponse)
                Assert.Equal(90, responseBody.Length);
        }

        [Theory]
        [InlineData("AAdmin12!", true)]
        [InlineData("wrongPassword", false)]
        public async Task IsAuthenticationPossible(string password, bool sucessfulResponse)
        {
            var client = _factory.CreateClient();

            var challangeResponse = await client.GetAsync("api/auth/Admin");
            string challangeBody = await challangeResponse.Content.ReadAsStringAsync();
            string salt = challangeBody.Trim('"');

            var hashedPassword = SHA.GenerateSHA256String(password);
            var hashedWithSalt = SHA.ComputePasswordAndSalt(hashedPassword, salt, SHA256.Create());
            
            var jsonString = JsonConvert.SerializeObject(hashedWithSalt);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var authResponse = await client.PostAsync("api/auth/Admin", content);
            string authBody = await authResponse.Content.ReadAsStringAsync();

            Assert.Equal(sucessfulResponse, authResponse.IsSuccessStatusCode);

            if(sucessfulResponse)
                Assert.Equal(269, authBody.Length);
        }

        [Fact]
        public async Task AuthenticationShouldFailWithoutSaltFirst()
        {
            var client = _factory.CreateClient();

            var jsonString = JsonConvert.SerializeObject("test");
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var authResponse = await client.PostAsync("api/auth/Admin", content);

            Assert.False(authResponse.IsSuccessStatusCode);
        }
    }
}
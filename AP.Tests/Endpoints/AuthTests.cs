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
using AP.Tests.Helpers;
using AP.Web;

namespace AP.Tests.Endpoints
{
    public class AuthTests : BaseEndpointTest
    {
        public AuthTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("Admin", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public async Task IsSaltGeneratingCorrectly(string username, bool sucessfulResponse)
        {
            var response = await DefaultClient.GetAsync($"api/auth/{username}");
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
            var challangeResponse = await DefaultClient.GetAsync("api/auth/Admin");
            string challangeBody = await challangeResponse.Content.ReadAsStringAsync();
            string salt = challangeBody.Trim('"');

            var hashedPassword = SHA.GenerateSHA256String(password);
            var hashedWithSalt = SHA.ComputePasswordAndSalt(hashedPassword, salt, SHA256.Create());
            
            var jsonString = JsonConvert.SerializeObject(hashedWithSalt);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var authResponse = await DefaultClient.PostAsync("api/auth/Admin", content);
            string authBody = await authResponse.Content.ReadAsStringAsync();

            Assert.Equal(sucessfulResponse, authResponse.IsSuccessStatusCode);

            if(sucessfulResponse)
                Assert.Equal(269, authBody.Length);
        }

        [Fact]
        public async Task AuthenticationShouldFailWithoutSaltFirst()
        {
            var jsonString = JsonConvert.SerializeObject("test");
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var authResponse = await DefaultClient.PostAsync("api/auth/Admin", content);

            Assert.False(authResponse.IsSuccessStatusCode);
        }
    }
}
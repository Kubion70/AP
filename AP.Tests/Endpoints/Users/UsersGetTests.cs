using System;
using System.Net;
using System.Threading.Tasks;
using AP.Tests.Helpers;
using AP.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Models = AP.Entities.Models;

namespace AP.Tests.Endpoints.Users
{
    public class UsersGetTests : BaseEndpointTest
    {
        private static Models.User sampleUser = new Models.User
        {
            Username = "SampleUser",
            Password = "SampleUser12#",
            Email = "sample@user.com",
            FirstName = "Sample",
            LastName = "User"
        };

        public UsersGetTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
            sampleUser = userRepository.Create(sampleUser).Result;
        }

        [Fact]
        public async Task IsItPossibleToGetAllUsers()
        {
            var response = await DefaultClient.GetAsync("api/Users");
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetSpecificUser()
        {
            // Using bad guid
            var badResponse = await DefaultClient.GetAsync($"api/Users/{Guid.Empty}");
            Assert.Equal(HttpStatusCode.BadRequest, badResponse.StatusCode);

            // Using not existing guid
            var noContentResponse = await DefaultClient.GetAsync($"api/Users/{Guid.NewGuid()}");
            Assert.Equal(HttpStatusCode.NoContent, noContentResponse.StatusCode);

            // Using good guid
            var response = await DefaultClient.GetAsync($"api/Users/{sampleUser.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        } 
    }
}
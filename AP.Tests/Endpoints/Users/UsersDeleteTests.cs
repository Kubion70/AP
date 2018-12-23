using System;
using System.Net;
using System.Threading.Tasks;
using AP.Cryptography;
using AP.Tests.Helpers;
using AP.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Models = AP.Entities.Models;

namespace AP.Tests.Endpoints.Users
{
    public class UsersDeleteTests : BaseEndpointTest
    {
        Models.User sampleUser = new Models.User()
        {
            Username = "SampleUsername",
            Password = SHA.GenerateSHA256String("Testing"),
            Email = "Sample@username.com"
        };

        public UsersDeleteTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
            
            sampleUser = userRepository.Create(sampleUser).Result;
        }

        [Fact]
        public async Task IsDeleteOperationWorksCorrectly()
        {
            var unauthResponse = await DefaultClient.DeleteAsync($"api/Users/{sampleUser.Id}");
            Assert.Equal(HttpStatusCode.Unauthorized, unauthResponse.StatusCode);

            var badResponse = await AuthorizedClient.DeleteAsync($"api/Users/{Guid.Empty}");
            Assert.Equal(HttpStatusCode.BadRequest, badResponse.StatusCode);

            var noContentResponse = await AuthorizedClient.DeleteAsync($"api/Users/{Guid.NewGuid()}");
            Assert.Equal(HttpStatusCode.NoContent, noContentResponse.StatusCode);

            var response = await AuthorizedClient.DeleteAsync($"api/Users/{sampleUser.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
using System.Threading.Tasks;
using AP.Tests.Helpers;
using AP.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Models = AP.Entities.Models;
using Eager = AP.Entities.Models.Eager;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Net;
using System;

namespace AP.Tests.Endpoints.Users
{
    public class UsersPutTests : BaseEndpointTest
    {
        Models.User sampleUser = new Models.User
        {
            Username = "putUser",
            Password = "PutUser12#",
            Email = "put@user.com"
        };

        public UsersPutTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
            sampleUser = userRepository.Create(sampleUser).Result;
        }

        [Fact]
        public async Task IsUpdatingUserPossible()
        {
            var users = userRepository.GetAllUsers().Result;
            var goodData = new Eager.User
            {
                Id = sampleUser.Id,
                Username = "NewUsername",
                Password = "NewPassword12!",
                Email = "user@put.net"
            };

            var badData = new Eager.User
            {
                Username = "",
                Password = "qwerty",
                Email = "user@put"
            };

            var contentGoodData = new StringContent(JsonConvert.SerializeObject(goodData), Encoding.UTF8, "application/json");
            var contentBadData = new StringContent(JsonConvert.SerializeObject(badData), Encoding.UTF8, "application/json");
            
            badData.Id = Guid.NewGuid();
            var contentUnknownId = new StringContent(JsonConvert.SerializeObject(badData), Encoding.UTF8, "application/json");

            var unauthResponse = await DefaultClient.PutAsync("api/Users", contentGoodData);
            Assert.Equal(HttpStatusCode.Unauthorized, unauthResponse.StatusCode);

            var badResponse = await AuthorizedClient.PutAsync("api/Users", contentBadData);
            Assert.Equal(HttpStatusCode.BadRequest, badResponse.StatusCode);

            var noContentResponse = await AuthorizedClient.PutAsync($"api/Users", contentUnknownId);
            Assert.Equal(HttpStatusCode.NoContent, noContentResponse.StatusCode);

            var goodResponse = await AuthorizedClient.PutAsync("api/Users", contentGoodData);
            Assert.Equal(HttpStatusCode.OK, goodResponse.StatusCode);
        }
    }
}
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AP.Tests.Helpers;
using AP.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;
using Eager = AP.Entities.Models.Eager;

namespace AP.Tests.Endpoints.Users
{
    public class UsersPostTests : BaseEndpointTest
    {
        public UsersPostTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task IsItPossibleToCreateNewUser()
        {
            var user = new Eager.User()
            {
                Username = "PostUser",
                Password = "PostUser12!",
                Email = "post@user.com"
            };
            var content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

            var userBadData = new Eager.User()
            {
                Username = "",
                Password = "qwerty",
                Email = "post@user"
            };
            var contentBadData = new StringContent(JsonConvert.SerializeObject(userBadData), Encoding.UTF8, "application/json");

            var unauthResponse = await DefaultClient.PostAsync("api/Users", content);
            Assert.Equal(HttpStatusCode.Unauthorized, unauthResponse.StatusCode);

            var badResponse = await AuthorizedClient.PostAsync("api/Users", contentBadData);
            Assert.Equal(HttpStatusCode.BadRequest, badResponse.StatusCode);

            var response = await AuthorizedClient.PostAsync("api/Users", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}
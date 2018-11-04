using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AP.Cryptography;
using AP.Repositories.User;
using AP.Tests.Helpers;
using AP.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;
using Models = AP.Entities.Models;

namespace AP.Tests.Endpoints.Categories
{
    public class CategoriesPostsTests : BaseEndpointTest
    {
        public CategoriesPostsTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("Test", HttpStatusCode.Created)]
        [InlineData("", HttpStatusCode.BadRequest)]
        public async Task CheckIfCreatingCategoryIsCorret(string categoryName, HttpStatusCode responseStatus)
        {
            var category = new Models.Eager.Category()
            {
                Name = categoryName
            };
            var jsonString = JsonConvert.SerializeObject(category);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            
            var response = await AuthorizedClient.PostAsync("api/Categories", content);

            Assert.Equal(responseStatus, response.StatusCode);
        }

        [Fact]
        public async Task CreatingCategoryWithoutAuthorization()
        {
            var category = new Models.Eager.Category()
            {
                Name = "No auth test name"
            };
            var jsonString = JsonConvert.SerializeObject(category);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            
            var response = await DefaultClient.PostAsync("api/Categories", content);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
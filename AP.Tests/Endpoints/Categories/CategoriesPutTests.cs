using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AP.Tests.Helpers;
using AP.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;
using Models = AP.Entities.Models;

namespace AP.Tests.Endpoints.Categories
{
    public class CategoriesPutTests : BaseEndpointTest
    {
        public CategoriesPutTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task CategoryPutOperationsTest()
        {
            var category = new Models.Category()
            {
                Name = "PutTest"
            };
            category = await categoryRepository.Create(category);

            var categroyUpdateModel = new Models.Eager.Category()
            {
                Id = category.Id,
                Name = "PutUpdated"
            };
            var updateContent = new StringContent(JsonConvert.SerializeObject(categroyUpdateModel), Encoding.UTF8, "application/json");

            var categoryWithEmptyId = new Models.Eager.Category()
            {
                Name = "PutUpdated"
            };
            var contentNoId = new StringContent(JsonConvert.SerializeObject(categoryWithEmptyId), Encoding.UTF8, "application/json");

            var categoryWithWrongId = new Models.Eager.Category()
            {
                Id = Guid.NewGuid(),
                Name = "PutUpdated"
            };
            var contentWrongId = new StringContent(JsonConvert.SerializeObject(categoryWithWrongId), Encoding.UTF8, "application/json");


            var unauthorizedResponse = await DefaultClient.PutAsync($"api/Categories", updateContent);
            Assert.Equal(HttpStatusCode.Unauthorized, unauthorizedResponse.StatusCode);

            var badResponse  = await AuthorizedClient.PutAsync($"api/Categories", contentNoId);
            Assert.Equal(HttpStatusCode.BadRequest, badResponse.StatusCode);

            var noContentResponse = await AuthorizedClient.PutAsync($"api/Categories", contentWrongId);
            Assert.Equal(HttpStatusCode.NoContent, noContentResponse.StatusCode);

            var response = await AuthorizedClient.PutAsync($"api/Categories", updateContent);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
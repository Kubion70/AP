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

            Models.Eager.Category[] categroyUpdateModel = {
                new Models.Eager.Category()
                {
                    Id = category.Id,
                    Name = "updateContent"
                }
            };
            var updateContent = new StringContent(JsonConvert.SerializeObject(categroyUpdateModel), Encoding.UTF8, "application/json");

            Models.Eager.Category[] categoryWithEmptyId = {
                new Models.Eager.Category()
                {
                    Name = "contentNoId"
                }
            };
            var contentNoId = new StringContent(JsonConvert.SerializeObject(categoryWithEmptyId), Encoding.UTF8, "application/json");

            Models.Eager.Category[] categoryWithWrongId = {
                new Models.Eager.Category()
                {
                    Id = Guid.NewGuid(),
                    Name = "contentWrongId"
                }
            };
            var contentWrongId = new StringContent(JsonConvert.SerializeObject(categoryWithWrongId), Encoding.UTF8, "application/json");


            var unauthorizedResponse = await DefaultClient.PutAsync($"api/Categories", updateContent);
            Assert.Equal(HttpStatusCode.Unauthorized, unauthorizedResponse.StatusCode);

            // When no Id create new record
            var createResponse = await AuthorizedClient.PutAsync($"api/Categories", contentNoId);
            Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

            // When Id is invalid (e.g not found in db)
            var badRequestResponse = await AuthorizedClient.PutAsync($"api/Categories", contentWrongId);
            Assert.Equal(HttpStatusCode.BadRequest, badRequestResponse.StatusCode);
        }
    }
}
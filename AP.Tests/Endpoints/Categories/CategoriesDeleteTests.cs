using System;
using System.Net;
using System.Threading.Tasks;
using AP.Repositories.Category;
using AP.Tests.Helpers;
using AP.Web;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Models = AP.Entities.Models;

namespace AP.Tests.Endpoints.Categories
{
    public class CategoriesDeleteTests : BaseEndpointTest
    {
        public CategoriesDeleteTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task CategoryDeleteOperationsTest()
        {
            var unauthorizedResponse = await DefaultClient.DeleteAsync($"api/Categories/{Guid.NewGuid()}");
            Assert.Equal(HttpStatusCode.Unauthorized, unauthorizedResponse.StatusCode);

            var notExistingResponse = await AuthorizedClient.DeleteAsync($"api/Categories/{Guid.NewGuid()}");
            Assert.Equal(HttpStatusCode.NoContent, notExistingResponse.StatusCode);

            var badResponse = await AuthorizedClient.DeleteAsync($"api/Categories/{Guid.Empty}");
            Assert.Equal(HttpStatusCode.BadRequest, badResponse.StatusCode);

            var category = new Models.Category()
            {
                Name = "IsDeletePossible"
            };
            category = await categoryRepository.Create(category);

            var response = await AuthorizedClient.DeleteAsync($"api/Categories/{category.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
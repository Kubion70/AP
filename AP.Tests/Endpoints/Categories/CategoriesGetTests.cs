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
    public class CategoriesGetTests : BaseEndpointTest
    {
        public CategoriesGetTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetAllCategories()
        {
            // Add sample category
            var category = new Models.Category()
            {
                Name = "Test"
            };
            await categoryRepository.Create(category);

            // Response should return 
            var getAllResponse = await DefaultClient.GetAsync("api/Categories");
            Assert.Equal(HttpStatusCode.OK, getAllResponse.StatusCode);
        }

        [Fact]
        public async Task GetOneCategory()
        {
            // If we pass empty guid the should be bad request response
            var badResponse = await DefaultClient.GetAsync($"api/Categories/{Guid.Empty}");
            Assert.Equal(HttpStatusCode.BadRequest, badResponse.StatusCode);

            // Add sample category
            var category = new Models.Category()
            {
                Name = "Test"
            };
            category = await categoryRepository.Create(category);

            // After category added check if record can be returned
            var response = await DefaultClient.GetAsync($"api/Categories/{category.Id}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AP.Cryptography;
using AP.Repositories.Category;
using AP.Repositories.Post;
using AP.Repositories.User;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Models = AP.Entities.Models;

namespace AP.Tests.Helpers
{
    public abstract class BaseEndpointTest : IClassFixture<WebApplicationFactory<AP.Web.Startup>>
    {
        private readonly WebApplicationFactory<AP.Web.Startup> factory;

        protected HttpClient DefaultClient 
        { 
            get 
            { 
                return factory.CreateClient(); 
            }
        }

        protected HttpClient AuthorizedClient 
        { 
            get 
            { 
                CreateAdminUser().Wait();
                var client = factory.CreateClient(); 
                client.DefaultRequestHeaders.Authorization = AuthorizationHelper.AdminAuthorization(client).Result;
                return client;
            }
        }

        protected readonly IUserRepository userRepository;
        protected readonly ICategoryRepository categoryRepository;
        protected readonly IPostRepository postRepository;

        protected BaseEndpointTest(WebApplicationFactory<AP.Web.Startup> factory)
        {
            this.factory = factory;

            userRepository = new UserRepository();
            categoryRepository = new CategoryRepository();
            postRepository = new PostRepository();
        }

        protected async Task CreateAdminUser()
        {
            var users = await userRepository.GetAllUsers();

            if(!users.Any())
            {
                var adminUser = new Models.User()
                {
                    Username = "Admin",
                    Password = SHA.GenerateSHA256String("AAdmin12!"),
                    Email = "admin@admin.pl",
                    FirstName = "Adam",
                    LastName = "Adamowski"
                };

                await userRepository.Create(adminUser);
            }
        }
    }
}
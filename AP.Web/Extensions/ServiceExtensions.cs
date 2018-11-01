using AP.Repositories.Category;
using AP.Repositories.Post;
using AP.Repositories.User;
using CacheManager.Core;
using Microsoft.Extensions.DependencyInjection;

namespace AP.Web.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<IPostRepository, PostRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();

            services.AddSingleton(CacheFactory.Build<string>("challangeCache", settings => 
            {
                settings.WithDictionaryHandle();
            }));

            return services;
        }
    }
}
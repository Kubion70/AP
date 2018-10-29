using AP.Repositories.Post;
using AP.Repositories.User;
using Microsoft.Extensions.DependencyInjection;

namespace AP.Web.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<IPostRepository, PostRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            return services;
        }
    }
}
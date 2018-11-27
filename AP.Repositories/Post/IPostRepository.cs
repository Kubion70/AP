using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AP.Entities.Options;
using AP.Repositories.Common;
using Models = AP.Entities.Models;

namespace AP.Repositories.Post
{
    public interface IPostRepository : IRepositoryBase<Models.Post>
    {
        Task<IEnumerable<Models.Post>> GetPosts();
        
        Task<IEnumerable<Models.Post>> GetPosts(PagingOptions pagingOptions);

        Task<IEnumerable<Models.Post>> GetPosts(Conditions<Models.Post> conditions);

        Task<IEnumerable<Models.Post>> GetPosts(PagingOptions pagingOptions, Conditions<Models.Post> conditions);

        Task<Models.Post> GetPostBySlug(string slug);

        Task<Models.Post> GetPostsById(Guid id);

        Task<int> CountAllPosts();

        Task<int> GetPostsVisitsSum(Conditions<Models.Post> conditions);
    }
}
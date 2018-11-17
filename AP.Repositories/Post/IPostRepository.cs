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
         Task<IEnumerable<Models.Post>> GetPostsByPagingOptions(PagingOptions pagingOptions);

         Task<Models.Post> GetPostBySlug(string slug);

         Task<Models.Post> GetPostsById(Guid id);

         Task<int> CountAllPosts();
    }
}
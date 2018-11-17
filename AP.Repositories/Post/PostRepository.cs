using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AP.Entities.Options;
using AP.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Models = AP.Entities.Models;

namespace AP.Repositories.Post
{
    public class PostRepository : RepositoryBase<Models.Post>, IPostRepository
    {
        public async Task<IEnumerable<Models.Post>> GetPostsByPagingOptions(PagingOptions pagingOptions)
        {
            if(pagingOptions == null)
                throw new ArgumentNullException(nameof(pagingOptions));
            
            pagingOptions.Offset = pagingOptions.Offset ?? 0;
            pagingOptions.Limit = pagingOptions.Limit ?? 100;

            var posts = _databaseContext.Posts.Skip(pagingOptions.Offset.Value).Take(pagingOptions.Limit.Value).Include(p => p.Author).Include(p => p.PostCategories);
            return await posts.ToListAsync();
        }

        public async Task<Models.Post> GetPostBySlug(string slug)
        {
            if(string.IsNullOrWhiteSpace(slug))
            {
                return null;
            }

            var post = await _databaseContext.Posts.Where(p => p.Slug.Equals(slug)).Include(p => p.Author).Include(p => p.PostCategories).SingleOrDefaultAsync();

            return post;
        }

        public async Task<Models.Post> GetPostsById(Guid id)
        {
            if(id.Equals(Guid.Empty))
            {
                return null;
            }

            var post = await _databaseContext.Posts.Where(p => p.Id.Equals(id)).Include(p => p.Author).Include(p => p.PostCategories).SingleOrDefaultAsync();

            return post;
        }

        public async Task<int> CountAllPosts()
        {
            return await _databaseContext.Posts.CountAsync();
        }
    }
}
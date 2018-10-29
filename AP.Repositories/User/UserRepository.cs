using System;
using System.Linq;
using System.Threading.Tasks;
using AP.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Models = AP.Entities.Models;

namespace AP.Repositories.User
{
    public class UserRepository : RepositoryBase<Models.User>, IUserRepository
    {
        public async Task<Models.User> GetUserByPostId(Guid postId)
        {
            if (postId == Guid.Empty)
                throw new ArgumentNullException(nameof(postId));

            var post = await _databaseContext.Posts.Include(p => p.Author).Where(p => p.Id.Equals(postId)).FirstOrDefaultAsync();

            return post?.Author;
        }
    }
}

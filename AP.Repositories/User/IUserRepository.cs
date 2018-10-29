using System;
using System.Threading.Tasks;
using AP.Repositories.Common;
using Models = AP.Entities.Models;

namespace AP.Repositories.User
{
    public interface IUserRepository : IRepositoryBase<Models.User>
    {
        Task<Models.User> GetUserByPostId(Guid postId);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AP.Repositories.Common;
using Models = AP.Entities.Models;

namespace AP.Repositories.User
{
    public interface IUserRepository : IRepositoryBase<Models.User>
    {
        Task<IEnumerable<Models.User>> GetAllUsers();

        Task<Models.User> GetUserByPostId(Guid postId);

        Task<Models.User> GetUserByUsername(string username);
    }
}

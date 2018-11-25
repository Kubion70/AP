using System.Collections.Generic;
using System.Threading.Tasks;
using AP.Entities.Options;
using AP.Repositories.Common;
using Models = AP.Entities.Models;

namespace AP.Repositories.Category
{
    public interface ICategoryRepository : IRepositoryBase<Models.Category>
    {
        Task<IEnumerable<Models.Category>> GetAllCategories();

        Task<IEnumerable<Models.Category>> GetCategoriesWithConditions(Conditions<Models.Category> conditions);

        Task<bool> MassCategoriesUpdate(IEnumerable<Models.Category> categories);
    }
}
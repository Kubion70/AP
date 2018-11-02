using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AP.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Models = AP.Entities.Models;

namespace AP.Repositories.Category
{
    public class CategoryRepository : RepositoryBase<Models.Category>, ICategoryRepository
    {
        public async Task<IEnumerable<Models.Category>> GetAllCategories()
        {
            return await _databaseContext.Categories.ToListAsync();
        }
    }
}
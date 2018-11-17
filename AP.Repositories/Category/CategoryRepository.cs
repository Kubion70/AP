using System;
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

        public async Task<bool> MassCategoriesUpdate(IEnumerable<Models.Category> categories)
        {
            using (var transaction = await _databaseContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var currentCategories = await _databaseContext.Categories.ToListAsync();

                    if(categories.Any(c => string.IsNullOrWhiteSpace(c.Name)))
                        throw new ArgumentNullException("One of names is empty");

                    var categoriesToCreate = categories
                        .Where(c => c.Id == null || c.Id.Equals(Guid.Empty))
                        .Select(c => 
                        {
                            c.Name = c.Name;
                            c.CreatedOn = DateTime.Now; 
                            c.ModifiedOn = null;
                            return c;
                        });
                    Console.WriteLine(categoriesToCreate.Count());
                    await _databaseContext.Categories.AddRangeAsync(categoriesToCreate);

                    var categoriesToUpdate = categories
                    .Where(c => c.Id != null && !c.Id.Equals(Guid.Empty))
                    .Select(c => new Models.Category(c.Id)
                    {
                        Name = c.Name,
                        CreatedOn = c.CreatedOn,
                        ModifiedOn = DateTime.Now
                    });
                    foreach (var category in categoriesToUpdate)
                    {
                        var dbCat = await _databaseContext.Categories.FindAsync(category.Id);
                        _databaseContext.Entry(dbCat).CurrentValues.SetValues(category);
                    }

                    var categoriesToDelete = currentCategories.Where(c => !categories.Any(a => a.Id.Equals(c.Id)));
                    _databaseContext.Categories.RemoveRange(categoriesToDelete);

                    await _databaseContext.SaveChangesAsync();

                    transaction.Commit();
                    return true;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    transaction.Rollback();
                    return false;
                }
            }
        }
    }
}
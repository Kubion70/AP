using AP.Entities;
using AP.Repositories.Contexts;
using System;
using System.Threading.Tasks;

namespace AP.Repositories.Common
{
    public abstract class RepositoryBase<E> : IRepositoryBase<E> where E : Entity
    {
        internal DatabaseContext _databaseContext;

        protected RepositoryBase()
        {
            _databaseContext = new DatabaseContext();
        }

        public virtual async Task<E> Create(E entity)
        {
            // Adding value asynchrously
            var addTask = await _databaseContext.Set<E>().AddAsync(entity);
            entity = addTask.Entity;

            // Save added value asynchrously
            var saveTask = _databaseContext.SaveChangesAsync();
            saveTask.Start();

            return entity;
        }

        public virtual async Task<E> Retrieve(Guid id)
        {
            E entity = await _databaseContext.Set<E>().FindAsync(id);

            return entity;
        }

        public virtual async Task<E> Update(E entity)
        {
            E result = await _databaseContext.Set<E>().FindAsync(entity.Id);
            _databaseContext.Entry(result).CurrentValues.SetValues(entity);

            var saveTask = _databaseContext.SaveChangesAsync();
            saveTask.Start();

            return result;
        }

        public virtual async Task<Boolean> Delete(E entity)
        {
            E result = await _databaseContext.Set<E>().FindAsync(entity.Id);
            _databaseContext.Set<E>().Remove(result);

            return _databaseContext.SaveChanges() > 0;
        }
    }
}

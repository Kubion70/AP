using AP.Entities;
using AP.Repositories.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AP.Repositories.Common
{
    public abstract class RepositoryBase<E> : IRepositoryBase<E> where E : Entity
    {
        internal DatabaseContext _databaseContext;

        protected RepositoryBase(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        /// <summary>
        /// Add record to database
        /// </summary>
        public virtual async Task<E> Create(E entity)
        {
            // Adding value asynchrously
            var addTask = _databaseContext.Set<E>().Attach(entity);
            entity = addTask.Entity;
            
            // Save added value asynchrously
            var saveTask = await _databaseContext.SaveChangesAsync();

            return entity;
        }

        /// <summary>
        /// Retrieve record from database
        /// </summary>
        public virtual async Task<E> Retrieve(Guid id)
        {
            E entity = await _databaseContext.Set<E>().FindAsync(id);

            return entity;
        }

        /// <summary>
        /// Update exisitng record
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<E> Update(E entity)
        {
            E result = await _databaseContext.Set<E>().FindAsync(entity.Id);
            _databaseContext.Entry(result).CurrentValues.SetValues(entity);

            var saveTask = await _databaseContext.SaveChangesAsync();
            
            return result;
        }

        /// <summary>
        /// Remove exisiting record
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<Boolean> Delete(Guid entityId)
        {
            E result = await _databaseContext.Set<E>().FindAsync(entityId);
            _databaseContext.Set<E>().Remove(result);

            return _databaseContext.SaveChanges() > 0;
        }

        /// <summary>
        /// Returns whether record of specified id exists in database or not
        /// </summary>
        /// <param name="id">Entity unique identifier</param>
        public virtual bool Exists(Guid entityId)
        {
            if (entityId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(entityId));

            bool exists = _databaseContext.Set<E>().Any(e => e.Id.Equals(entityId));
            return exists;
        }

        public async Task<bool> RemoveRelation<R>(R relation) where R : class
        {
            _databaseContext.Set<R>().Remove(relation);

            return _databaseContext.SaveChanges() > 0;
        }

        public async Task<R> CreateRelation<R>(R relation) where R : class
        {
            // Adding value asynchrously
            var addTask = await _databaseContext.Set<R>().AddAsync(relation);
            relation = addTask.Entity;
            
            // Save added value asynchrously
            var saveTask = await _databaseContext.SaveChangesAsync();

            return relation;
        }
    }
}

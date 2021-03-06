using AP.Entities;
using System;
using System.Threading.Tasks;

namespace AP.Repositories.Common
{
    public interface IRepositoryBase<E> where E : Entity
    {
        Task<E> Create(E entity);

        Task<E> Retrieve(Guid id);

        Task<E> Update(E entity);

        Task<Boolean> Delete(Guid entityId);

        bool Exists(Guid entityId);

        Task<Boolean> RemoveRelation<R>(R relation) where R : class;

        Task<R> CreateRelation<R>(R relation) where R : class;
    }
}

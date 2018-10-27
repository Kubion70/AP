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

        Task<Boolean> Delete(E entity);
    }
}

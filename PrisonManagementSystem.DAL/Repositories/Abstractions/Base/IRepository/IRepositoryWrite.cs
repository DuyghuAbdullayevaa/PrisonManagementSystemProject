using PrisonManagementSystem.DAL.Entities.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Abstractions.Base.IRepository
{
    public interface IRepositoryWrite<T> : IRepositoryBase where T : BaseEntity
    {
        Task<bool> AddAsync(T entity);
        Task<bool> AddRangeAsync(List<T> entities);
        Task<bool> HardDeleteAsync(Guid id);
        Task<bool> HardDeleteAsync(T entity);
        Task<bool> RemoveRangeAsync(IEnumerable<T> entities);
        Task<bool> UpdateAsync(T entity);
        Task<bool> UpdateRangeAsync(IEnumerable<T> entities);
        Task<bool> SoftDeleteAsync(T entity);
        Task<bool> SoftDeleteAsync(Guid id);
        Task<bool> DeleteAsync(T entity, bool isHardDelete);
    }
}

using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.DAL.Entities.Base;

namespace PrisonManagementSystem.DAL.Repositories.Abstractions.Base.IRepository
{
    public interface IRepository<T> : IRepositoryBase
        where T : BaseEntity
    {
        DbSet<T> Table { get; }
    }
}


using PrisonManagementSystem.DAL.Repositories.Abstractions.Base.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Abstractions.Base
{
    public interface IUnitOfWork : IDisposable
    {
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task CommitAsync();
        TRepository GetRepository<TRepository>() where TRepository : class, IRepositoryBase;
     
    }
}



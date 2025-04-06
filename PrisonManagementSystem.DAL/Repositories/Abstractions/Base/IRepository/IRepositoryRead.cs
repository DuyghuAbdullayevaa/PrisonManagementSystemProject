using Microsoft.EntityFrameworkCore.Query;
using PrisonManagementSystem.DAL.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Abstractions.Base.IRepository
{
    public interface IRepositoryRead<T> : IRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(
            Guid id,
            bool enableTracking = false,
            bool isDeleted = false);

        Task<T?> GetSingleAsync(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            bool enableTracking = false,
            bool isDeleted = false);

        Task<IList<T>> GetAllAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            bool enableTracking = false,
            bool isDeleted = false);

        Task<IList<T>> GetAllByPagingAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            bool enableTracking = false,
            bool isDeleted = false,
            int currentPage = 1,
            int pageSize = 10);

        Task<int> GetCountAsync(
            Expression<Func<T, bool>>? predicate = null,
            bool isDeleted = false);

        Task<bool> AnyAsync(
            Expression<Func<T, bool>> predicate,
            bool isDeleted = false);
    }
}

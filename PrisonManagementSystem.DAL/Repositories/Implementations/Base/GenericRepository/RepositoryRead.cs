using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.Base;
using PrisonManagementSystem.DAL.Repositories.Abstractions.Base.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

public class RepositoryRead<T> : IRepositoryRead<T> where T : BaseEntity
{
    private readonly PrisonDbContext _context;
    private readonly DbSet<T> _dbSet;

    public RepositoryRead(PrisonDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public DbSet<T> Table => _dbSet;

    public async Task<T?> GetByIdAsync(Guid id, bool enableTracking = false, bool isDeleted = false)
    {
        IQueryable<T> queryable = Table;
        if (!enableTracking) queryable = queryable.AsNoTracking();
        if (isDeleted) queryable = queryable.IgnoreQueryFilters();
        return await queryable.FirstOrDefaultAsync(entity => entity.Id == id);
    }

    public async Task<T?> GetSingleAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        bool enableTracking = false,
        bool isDeleted = false)
    {
        IQueryable<T> queryable = Table;
        if (!enableTracking) queryable = queryable.AsNoTracking();
        if (isDeleted) queryable = queryable.IgnoreQueryFilters();
        if (include is not null) queryable = include(queryable);
        return await queryable.FirstOrDefaultAsync(predicate);
    }

    public async Task<IList<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool enableTracking = false,
        bool isDeleted = false)
    {
        IQueryable<T> queryable = Table;
        if (!enableTracking) queryable = queryable.AsNoTracking();
        if (isDeleted) queryable = queryable.IgnoreQueryFilters();
        if (include is not null) queryable = include(queryable);
        if (predicate is not null) queryable = queryable.Where(predicate);
        if (orderBy is not null) return await orderBy(queryable).ToListAsync();
        return await queryable.ToListAsync();
    }

    public async Task<IList<T>> GetAllByPagingAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool enableTracking = false,
        bool isDeleted = false,
        int currentPage = 1,
        int pageSize = 10)
    {
        IQueryable<T> queryable = Table;
        if (!enableTracking) queryable = queryable.AsNoTracking();
        if (isDeleted) queryable = queryable.IgnoreQueryFilters();
        if (include is not null) queryable = include(queryable);
        if (predicate is not null) queryable = queryable.Where(predicate);
        if (orderBy is not null)
            return await orderBy(queryable).Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
        return await queryable.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<int> GetCountAsync(Expression<Func<T, bool>>? predicate = null, bool isDeleted = false)
    {
        IQueryable<T> queryable = Table;
        if (isDeleted) queryable = queryable.IgnoreQueryFilters();
        if (predicate is not null) queryable = queryable.Where(predicate);
        return await queryable.CountAsync();
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, bool isDeleted = false)
    {
        IQueryable<T> queryable = Table;
        if (isDeleted) queryable = queryable.IgnoreQueryFilters();
        return await queryable.AnyAsync(predicate);
    }
}

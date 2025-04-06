using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.Base;
using PrisonManagementSystem.DAL.Repositories.Abstractions.Base.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.Base
{
    public class RepositoryWrite<T> : IRepositoryWrite<T> where T : BaseEntity
    {
        private readonly PrisonDbContext _context;
        private readonly DbSet<T> _table;

        public RepositoryWrite(PrisonDbContext context)
        {
            _context = context;
            _table = _context.Set<T>();
        }

        public async Task<bool> AddAsync(T entity)
        {
            await _table.AddAsync(entity);
            entity.CreateDate = DateTime.UtcNow;
            return true;
        }

        public async Task<bool> AddRangeAsync(List<T> entities)
        {
            await _table.AddRangeAsync(entities);
            foreach (var entity in entities)
            {
                entity.CreateDate = DateTime.UtcNow;
            }
            return true;
        }

        public async Task<bool> HardDeleteAsync(Guid id)
        {
            var entity = await _table.FindAsync(id);
            if (entity == null) return false;

            _table.Remove(entity);
            return true;
        }

        public async Task<bool> HardDeleteAsync(T entity)
        {
            _table.Remove(entity);
            return true;
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var entity = await _table.FindAsync(id);
            if (entity == null) return false;

            entity.IsDeleted = true;
            entity.DeleteDate = DateTime.UtcNow;
            _table.Update(entity);
            return true;
        }

        public async Task<bool> SoftDeleteAsync(T entity)
        {
            if (entity.IsDeleted) return false;

            entity.IsDeleted = true;
            entity.DeleteDate = DateTime.UtcNow;
            _table.Update(entity);
            return true;
        }

        public async Task<bool> DeleteAsync(T entity, bool isHardDelete)
        {
            if (isHardDelete)
            {
                return await HardDeleteAsync(entity);
            }
            else
            {
                return await SoftDeleteAsync(entity);
            }
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            entity.UpdateDate = DateTime.UtcNow;
            _table.Update(entity);
            return true;
        }

        public async Task<bool> UpdateRangeAsync(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                entity.UpdateDate = DateTime.UtcNow;
            }
            _table.UpdateRange(entities);
            return true;
        }

        public async Task<bool> RemoveRangeAsync(IEnumerable<T> entities)
        {
            _table.RemoveRange(entities);
            return true;
        }
    }
}

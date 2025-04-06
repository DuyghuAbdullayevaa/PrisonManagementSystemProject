using Microsoft.EntityFrameworkCore.Storage;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Repositories.Abstractions.Base;
using PrisonManagementSystem.DAL.Repositories.Abstractions.Base.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.Base
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PrisonDbContext _context;
        private readonly Dictionary<Type, IRepositoryBase> _repositories;
        private IDbContextTransaction _transaction;

        public UnitOfWork(PrisonDbContext context)
        {
            _context = context;
            _repositories = new Dictionary<Type, IRepositoryBase>();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
            }
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public TRepository GetRepository<TRepository>() where TRepository : class, IRepositoryBase
        {
            Type repositoryImplementationType = GetConcreteRepositoryInfo<TRepository>();
            if (_repositories.TryGetValue(repositoryImplementationType, out IRepositoryBase existingRepository))
            {
                return (TRepository)existingRepository;
            }

            IRepositoryBase repositoryInstance = (IRepositoryBase)Activator.CreateInstance(repositoryImplementationType, _context);

            if (repositoryInstance is TRepository repository)
            {
                _repositories.Add(repositoryImplementationType, repository);
                return repository;
            }

            throw new InvalidOperationException($"Could not create repository of type {repositoryImplementationType.FullName}");
        }

        private Type GetConcreteRepositoryInfo<TRepository>()
        {
            string interfaceName = typeof(TRepository).Name;
            string className = interfaceName.StartsWith("I") ? interfaceName.Substring(1) : interfaceName;

            string interfaceNamespace = typeof(TRepository).Namespace;
            string implementationFullName = $"{interfaceNamespace}.{className}";
            Type repositoryType = Type.GetType(implementationFullName);
            if (repositoryType == null)
            {
                repositoryType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.Name == className && typeof(TRepository).IsAssignableFrom(t));
            }
            if (repositoryType == null)
            {
                throw new InvalidOperationException($"No concrete repository class found for {typeof(TRepository).Name}");
            }

            return repositoryType;
        }
    }
}

using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.ICrimeRepository;
using PrisonManagementSystem.DAL.Repositories.Implementations.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.CrimeRepository
{
    public class CrimeWriteRepository : RepositoryWrite<Crime>, ICrimeWriteRepository
    {
        private readonly PrisonDbContext _context;

        public CrimeWriteRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.ICrimeRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.CrimeRepository
{
    public class CrimeReadRepository : RepositoryRead<Crime>, ICrimeReadRepository
    {
        private readonly PrisonDbContext _context;

        public CrimeReadRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }

       

    }
}

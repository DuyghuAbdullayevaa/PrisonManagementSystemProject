using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonerRepository;
using PrisonManagementSystem.DAL.Repositories.Implementations.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.PrisonerRepository
{
    internal class PrisonerWriteRepository : RepositoryWrite<Prisoner>, IPrisonerWriteRepository
    {
        private readonly PrisonDbContext _context;

        public PrisonerWriteRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }
    }
}

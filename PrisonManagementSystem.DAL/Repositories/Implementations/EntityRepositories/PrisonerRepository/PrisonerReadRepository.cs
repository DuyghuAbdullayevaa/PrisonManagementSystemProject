using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonerRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.PrisonerRepository
{
    public class PrisonerReadRepository : RepositoryRead<Prisoner>, IPrisonerReadRepository
    {
        private readonly PrisonDbContext _context;

        public PrisonerReadRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }
      


    }
}

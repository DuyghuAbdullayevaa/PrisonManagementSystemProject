using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonerCrimeRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonerIncidentRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.PrisonerCrimeRepository
{
    internal class PrisonerCrimeReadRepository : RepositoryRead<PrisonerCrime>, IPrisonerCrimeReadRepository
    {
        private readonly PrisonDbContext _context;

        public PrisonerCrimeReadRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }
    }
}

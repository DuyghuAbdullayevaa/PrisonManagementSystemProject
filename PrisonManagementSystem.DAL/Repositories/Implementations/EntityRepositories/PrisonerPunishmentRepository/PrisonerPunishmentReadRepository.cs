using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.Prison;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonerIncidentRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonerPunishmentRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.PrisonerPunishmentRepository
{
    public class PrisonerPunishmentReadRepository : RepositoryRead<PrisonerPunishment>, IPrisonerPunishmentReadRepository
    {
        private readonly PrisonDbContext _context;

        public PrisonerPunishmentReadRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }
    }
}

using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.ICrimeRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IIncidentPunishmentRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.IncidentPunishmentRepository
{
    public class IncidentPunishmentReadRepository : RepositoryRead<IncidentPunishment>, IIncidentPunishmentReadRepository
    {
        private readonly PrisonDbContext _context;

        public IncidentPunishmentReadRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }

    }
}

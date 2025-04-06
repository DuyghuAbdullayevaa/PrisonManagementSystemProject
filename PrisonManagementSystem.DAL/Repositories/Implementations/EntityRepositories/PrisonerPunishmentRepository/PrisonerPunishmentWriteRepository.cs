using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.Prison;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonerPunishmentRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonerRepository;
using PrisonManagementSystem.DAL.Repositories.Implementations.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.PrisonerPunishmentRepository
{
    internal class PrisonerPunishmentWriteRepository : RepositoryWrite<PrisonerPunishment>, IPrisonerPunishmentWriteRepository
    {
        private readonly PrisonDbContext _context;

        public PrisonerPunishmentWriteRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }
    }
}

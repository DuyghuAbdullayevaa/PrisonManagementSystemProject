using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPunishmentRepository;
using PrisonManagementSystem.DAL.Repositories.Implementations.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.PunishmentRepository
{
    public class PunishmentWriteRepository : RepositoryWrite<Punishment>, IPunishmentWriteRepository
    {
        private readonly PrisonDbContext _context;

        public PunishmentWriteRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }
    }
}

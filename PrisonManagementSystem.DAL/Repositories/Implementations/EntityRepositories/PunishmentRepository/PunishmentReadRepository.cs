using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPunishmentRepository;
using PrisonManagementSystem.DAL.Repositories.Implementations.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.PunishmentRepository
{
    public class PunishmentReadRepository : RepositoryRead<Punishment>, IPunishmentReadRepository
    {
        private readonly PrisonDbContext _context;

        public PunishmentReadRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }



    }
}

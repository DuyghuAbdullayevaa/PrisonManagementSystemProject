using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonStaffRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPunishmentRepository;
using PrisonManagementSystem.DAL.Repositories.Implementations.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.PrisonStaffRepository
{
    
    public class PrisonStaffWriteRepository : RepositoryWrite<PrisonStaff>, IPrisonStaffWriteRepository
    {
        private readonly PrisonDbContext _context;

        public PrisonStaffWriteRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonStaffRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.PrisonStaffRepository
{
    internal class PrisonStaffReadRepository : RepositoryRead<PrisonStaff>, IPrisonStaffReadRepository
    {
        private readonly PrisonDbContext _context;

        public PrisonStaffReadRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }

    }
}

using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IStaffRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.StaffRepository
{
    public class StaffReadRepository : RepositoryRead<Staff>, IStaffReadRepository
    {
        private readonly PrisonDbContext _context;

        public StaffReadRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }

        
    }
}

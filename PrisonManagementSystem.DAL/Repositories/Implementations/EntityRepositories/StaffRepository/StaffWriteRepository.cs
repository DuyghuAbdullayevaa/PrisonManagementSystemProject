using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IStaffRepository;
using PrisonManagementSystem.DAL.Repositories.Implementations.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.StaffRepository
{
    public class StaffWriteRepository : RepositoryWrite<Staff>, IStaffWriteRepository
    {
        private readonly PrisonDbContext _context;

        public StaffWriteRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }
    }
}

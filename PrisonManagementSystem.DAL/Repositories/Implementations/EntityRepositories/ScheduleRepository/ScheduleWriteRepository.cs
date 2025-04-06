using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IScheduleRepository;
using PrisonManagementSystem.DAL.Repositories.Implementations.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.ScheduleRepository
{
    public class ScheduleWriteRepository : RepositoryWrite<Schedule>, IScheduleWriteRepository
    {
        private readonly PrisonDbContext _context;

        public ScheduleWriteRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }
    }
}

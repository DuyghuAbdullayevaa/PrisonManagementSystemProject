using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IScheduleRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.ScheduleRepository
{
    public class ScheduleReadRepository : RepositoryRead<Schedule>, IScheduleReadRepository
    {
        private readonly PrisonDbContext _context;

        public ScheduleReadRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }

       
    }
}

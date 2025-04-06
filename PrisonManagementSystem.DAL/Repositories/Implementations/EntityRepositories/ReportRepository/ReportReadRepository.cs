using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.Prison;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IReportRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.ReportRepository
{
    public class ReportReadRepository : RepositoryRead<Report>, IReportReadRepository
    {
        private readonly PrisonDbContext _context;

        public ReportReadRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }

       
    }
}

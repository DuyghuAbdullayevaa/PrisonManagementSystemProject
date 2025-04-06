using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.Prison;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IReportRepository;
using PrisonManagementSystem.DAL.Repositories.Implementations.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.ReportRepository
{
    public class ReportWriteRepository : RepositoryWrite<Report>, IReportWriteRepository
    {
        private readonly PrisonDbContext _context;

        public ReportWriteRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }
    }
}

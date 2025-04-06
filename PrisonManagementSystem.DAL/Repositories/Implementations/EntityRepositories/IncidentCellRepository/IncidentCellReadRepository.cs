using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IIncidentCellRepository;
using PrisonManagementSystem.DAL.Repositories.Implementations.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.IncidentCellRepository
{
    class IncidentCellReadRepository : RepositoryRead<IncidentCell>, IIncidentCellReadRepository
    {
        private readonly PrisonDbContext _context;

        public IncidentCellReadRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }
    
    }
}

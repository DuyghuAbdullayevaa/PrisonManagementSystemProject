using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IIncidentCellRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IIncidentRepository;
using PrisonManagementSystem.DAL.Repositories.Implementations.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.IncidentCellRepository
{
    public class IncidentCellWriteRepository : RepositoryWrite<IncidentCell>, IIncidentCellWriteRepository
    {
        private readonly PrisonDbContext _context;

        public IncidentCellWriteRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }
    }
}

using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IVisitRepository;
using PrisonManagementSystem.DAL.Repositories.Implementations.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.VisitRepository
{
    public class VisitWriteRepository : RepositoryWrite<Visit>, IVisitWriteRepository
    {
        private readonly PrisonDbContext _context;

        public VisitWriteRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }

    }
}

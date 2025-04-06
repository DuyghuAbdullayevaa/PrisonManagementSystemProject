using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IVisitorRepository;
using PrisonManagementSystem.DAL.Repositories.Implementations.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.VisitorRepository
{
    public class VisitorWriteRepository : RepositoryWrite<Visitor>, IVisitorWriteRepository
    {
        private readonly PrisonDbContext _context;

        public VisitorWriteRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IVisitorRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.VisitorRepository
{
    public class VisitorReadRepository : RepositoryRead<Visitor>, IVisitorReadRepository
    {
        private readonly PrisonDbContext _context;

        public VisitorReadRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }
      
    }
}

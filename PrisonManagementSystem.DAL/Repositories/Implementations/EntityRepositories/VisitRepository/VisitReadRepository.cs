using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IVisitRepository;
using PrisonManagementSystem.DAL.Repositories.Implementations.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.VisitRepository
{
    public class VisitReadRepository : RepositoryRead<Visit>, IVisitReadRepository
    {
        private readonly PrisonDbContext _context;

        public VisitReadRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }

    }
}

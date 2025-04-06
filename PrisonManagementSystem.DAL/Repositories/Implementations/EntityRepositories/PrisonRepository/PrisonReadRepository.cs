using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.PrisonRepository
{
    public class PrisonReadRepository : RepositoryRead<Prison>, IPrisonReadRepository
    {
        private readonly PrisonDbContext _context;

        public PrisonReadRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }
      

    }
}


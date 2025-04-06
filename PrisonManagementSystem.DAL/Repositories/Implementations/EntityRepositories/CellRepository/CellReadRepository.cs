using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Extensions;
using PrisonManagementSystem.DAL.Models;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.ICellRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.CellRepository
{
    public class CellReadRepository : RepositoryRead<Cell>, ICellReadRepository
    {
        private readonly PrisonDbContext _context;

        public CellReadRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }
       
    }
}

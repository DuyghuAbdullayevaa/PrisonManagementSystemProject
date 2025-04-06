using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.ICellRepository;
using PrisonManagementSystem.DAL.Repositories.Implementations.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.CellRepository
{
    public class CellWriteRepository : RepositoryWrite<Cell>, ICellWriteRepository
    {
        private readonly PrisonDbContext _context;

        public CellWriteRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }
    }
}



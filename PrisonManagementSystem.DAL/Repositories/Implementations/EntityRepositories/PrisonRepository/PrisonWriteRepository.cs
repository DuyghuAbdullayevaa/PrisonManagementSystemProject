using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonRepository;
using PrisonManagementSystem.DAL.Repositories.Implementations.Base;
using System.Linq;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.PrisonRepository
{
    public class PrisonWriteRepository : RepositoryWrite<Prison>, IPrisonWriteRepository
    {
        private readonly PrisonDbContext _context;

        public PrisonWriteRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }

      
    }
}

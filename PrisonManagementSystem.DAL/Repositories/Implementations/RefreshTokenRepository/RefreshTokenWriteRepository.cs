using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.Identity;
using PrisonManagementSystem.DAL.Repositories.Abstractions.IRefreshTokenRepository;
using PrisonManagementSystem.DAL.Repositories.Implementations.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.RefreshTokenRepository
{
    public class RefreshTokenWriteRepository : RepositoryWrite<RefreshToken>, IRefreshTokenWriteRepository
    {
        private readonly PrisonDbContext _context;

        public RefreshTokenWriteRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }

    }
}

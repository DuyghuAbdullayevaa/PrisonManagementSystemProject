using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.Identity;
using PrisonManagementSystem.DAL.Repositories.Abstractions.IRefreshTokenRepository;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.RefreshTokenRepository
{
    public class RefreshTokenReadRepository : RepositoryRead<RefreshToken>, IRefreshTokenReadRepository
    {
        private readonly PrisonDbContext _context;

        public RefreshTokenReadRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }

      
    }
}
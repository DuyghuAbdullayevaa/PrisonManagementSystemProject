using PrisonManagementSystem.DAL.Entities.Identity;
using PrisonManagementSystem.DAL.Repositories.Abstractions.Base.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Abstractions.IRefreshTokenRepository
{
    public interface IRefreshTokenWriteRepository : IRepositoryWrite<RefreshToken>
    {

    }
}

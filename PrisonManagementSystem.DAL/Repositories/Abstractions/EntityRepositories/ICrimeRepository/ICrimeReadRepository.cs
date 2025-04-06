using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DAL.Repositories.Abstractions.Base.IRepository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.ICrimeRepository
{
    public interface ICrimeReadRepository : IRepositoryRead<Crime>
    {
      
    }
}

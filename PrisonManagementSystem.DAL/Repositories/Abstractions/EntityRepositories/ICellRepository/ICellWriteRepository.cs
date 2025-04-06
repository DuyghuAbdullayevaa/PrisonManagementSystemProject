﻿using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Repositories.Abstractions.Base.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.ICellRepository
{
    public interface ICellWriteRepository : IRepositoryWrite<Cell>
    {
    }
}

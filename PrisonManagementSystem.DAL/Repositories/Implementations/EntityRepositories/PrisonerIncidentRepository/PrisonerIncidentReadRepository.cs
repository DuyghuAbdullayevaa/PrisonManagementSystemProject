﻿using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonerIncidentRepository;
using PrisonManagementSystem.DAL.Repositories.Abstractions.EntityRepositories.IPrisonerRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Repositories.Implementations.EntityRepositories.PrisonerIncidentRepository
{
    public class PrisonerIncidentReadRepository : RepositoryRead<PrisonerIncident>, IPrisonerIncidentReadRepository
    {
        private readonly PrisonDbContext _context;

        public PrisonerIncidentReadRepository(PrisonDbContext context) : base(context)
        {
            _context = context;
        }
    }
}

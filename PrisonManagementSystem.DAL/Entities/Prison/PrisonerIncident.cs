using PrisonManagementSystem.DAL.Entities.Base;
using System;

namespace PrisonManagementSystem.DAL.Entities.PrisonDBContext
{
    public class PrisonerIncident : BaseEntity
    {
        public Guid PrisonerId { get; set; }
        public Prisoner Prisoner { get; set; }

        public Guid IncidentId { get; set; }
        public Incident Incident { get; set; }
    }
}

using PrisonManagementSystem.DAL.Entities.Base;
using PrisonManagementSystem.DAL.Entities.Prison;
using PrisonManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;

namespace PrisonManagementSystem.DAL.Entities.PrisonDBContext
{
    public class Incident : BaseEntity
    {
        public string Description { get; set; }
        public DateTime IncidentDate { get; set; }
        public IncidentStatus Status { get; set; }
        public IncidentType Type { get; set; }
        public ICollection<PrisonerIncident> PrisonerIncidents { get; set; }
        public ICollection<IncidentPunishment> IncidentPunishments { get; set; }
        public ICollection<IncidentCell>? IncidentCells { get; set; }
        public Guid PrisonId { get; set; }
        public Prison Prison { get; set; }
        public ICollection<Report> Reports { get; set; }
    }
}

using PrisonManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.DTOs.Incident
{
    public class UpdateIncidentDto
    {
        public string Description { get; set; }
        public DateTime IncidentDate { get; set; }
        public IncidentType Type { get; set; }
        public IncidentStatus Status { get; set; }
        public List<Guid>? PrisonerIds { get; set; }
        public List<Guid>? CellIds { get; set; }
    }
}

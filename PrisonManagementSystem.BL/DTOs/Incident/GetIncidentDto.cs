using PrisonManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.DTOs.Incident
{
    public class GetIncidentDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateTime IncidentDate { get; set; }
        public IncidentStatus Status { get; set; }
        public IncidentType Type { get; set; }
        public string PrisonName { get; set; }
        public List<string> Prisoners { get; set; }
        public List<string> Cells { get; set; }
    }
}

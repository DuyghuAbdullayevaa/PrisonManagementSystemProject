using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DTOs;

namespace PrisonManagementSystem.BL.DTOs.Incident
{
    public class CreateIncidentDto
    {
        public string Description { get; set; }
        public DateTime IncidentDate { get; set; }
        public IncidentStatus Status { get; set; }
        public IncidentType Type { get; set; }
        public Guid PrisonId { get; set; }
        public List<Guid>? PrisonerIds { get; set; }
        public List<Guid>? CellIds { get; set; }
        public CreateReportDto Report { get; set; }
    }
}

using PrisonManagementSystem.DAL.Enums;

namespace PrisonManagementSystem.DTOs
{
    public class GetPunishmentDto
    {
        public Guid Id { get; set; }
        public List<string> PrisonerName { get; set; }
        public PunishmentType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<string>? IncidentDescriptions { get; set; }
    }
}

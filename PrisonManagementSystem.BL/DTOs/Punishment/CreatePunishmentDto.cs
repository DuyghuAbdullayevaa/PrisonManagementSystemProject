using PrisonManagementSystem.BL.DTOs.Punishment;
using PrisonManagementSystem.DAL.Enums;

namespace PrisonManagementSystem.DTOs
{
    public class CreatePunishmentDto
    {
        public Guid IncidentId { get; set; }
        public List<PunishmentAssignmentDto> Punishments { get; set; }
    }
}

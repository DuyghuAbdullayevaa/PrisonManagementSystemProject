using PrisonManagementSystem.DAL.Enums;
using System;

namespace PrisonManagementSystem.DTOs
{
    public class UpdatePunishmentDto
    {
        public Guid PrisonerId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid IncidentId { get; set; }
        public PunishmentType PunishmentType { get; set; }
    }
}

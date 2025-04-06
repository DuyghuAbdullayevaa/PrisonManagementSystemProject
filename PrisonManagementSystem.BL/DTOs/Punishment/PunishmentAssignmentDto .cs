using PrisonManagementSystem.DAL.Enums;
using System;

namespace PrisonManagementSystem.BL.DTOs.Punishment
{
    public class PunishmentAssignmentDto
    {
        public Guid PrisonerId { get; set; }
        public PunishmentType PunishmentType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

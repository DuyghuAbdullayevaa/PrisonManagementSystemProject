using PrisonManagementSystem.DAL.Enums;
using System;

namespace PrisonManagementSystem.BL.DTOs.Punishment
{
    public class PunishmentDto
    {
        public Guid Id { get; set; }
        public PunishmentType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

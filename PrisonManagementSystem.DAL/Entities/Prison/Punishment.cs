using PrisonManagementSystem.DAL.Entities.Base;
using PrisonManagementSystem.DAL.Entities.Prison;
using PrisonManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;

namespace PrisonManagementSystem.DAL.Entities.PrisonDBContext
{
    public class Punishment : BaseEntity
    {
        public ICollection<PrisonerPunishment> PrisonerPunishments { get; set; }
        public PunishmentType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ICollection<IncidentPunishment> IncidentPunishments { get; set; }
    }
}

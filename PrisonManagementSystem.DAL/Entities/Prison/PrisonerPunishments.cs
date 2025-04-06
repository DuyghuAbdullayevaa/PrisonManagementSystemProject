using PrisonManagementSystem.DAL.Entities.Base;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Entities.Prison
{
    public class PrisonerPunishment : BaseEntity
    {
        public Guid PrisonerId { get; set; }
        public Prisoner Prisoner { get; set; }

        public Guid PunishmentId { get; set; }
        public Punishment Punishment { get; set; }
    }
    

}

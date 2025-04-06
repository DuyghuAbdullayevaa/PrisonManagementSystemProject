using PrisonManagementSystem.DAL.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Entities.PrisonDBContext
{
    public class IncidentPunishment : BaseEntity
    {

        public Guid IncidentId { get; set; }
        public Incident Incident { get; set; }
        public Guid PunishmentId { get; set; }
        public Punishment Punishment { get; set; }
    

    }

}

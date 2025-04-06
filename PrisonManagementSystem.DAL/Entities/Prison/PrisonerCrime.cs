using PrisonManagementSystem.DAL.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Entities.PrisonDBContext
{

    public class PrisonerCrime : BaseEntity
    {

        public Guid PrisonerId { get; set; } 
        public Prisoner Prisoner { get; set; }

        public Guid CrimeId { get; set; } 
        public Crime Crime { get; set; }
    }

}

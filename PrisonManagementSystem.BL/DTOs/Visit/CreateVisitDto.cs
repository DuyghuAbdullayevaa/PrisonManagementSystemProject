using PrisonManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.DTOs.Visit
{
    namespace PrisonManagementSystem.DTOs
    {
        public class CreateVisitDto
        {
            public DateTime VisitDate { get; set; }
            public VisitType VisitType { get; set; }
            public int DurationInMinutes { get; set; }
            public Guid PrisonerId { get; set; }
            public Guid VisitorId { get; set; }  
        }

    }

}

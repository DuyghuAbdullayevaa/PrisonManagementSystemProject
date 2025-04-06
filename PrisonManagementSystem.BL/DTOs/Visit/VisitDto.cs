using PrisonManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.DTOs.Visit
{
    public class VisitDto
    {
        public DateTime VisitDate { get; set; }
        public int DurationInMinutes { get; set; }
        public VisitType VisitType { get; set; }  
        public Guid PrisonerId { get; set; }
    }
}

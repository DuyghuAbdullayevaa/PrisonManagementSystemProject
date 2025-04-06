using PrisonManagementSystem.DAL.Entities.Base;
using PrisonManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Entities.PrisonDBContext
{
    public class Schedule : BaseEntity
    {

        public ShiftType ShiftType { get; set; } 
        public DateTime Date { get; set; } 
        public Guid StaffId { get; set; } 
        public Staff Staff { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}

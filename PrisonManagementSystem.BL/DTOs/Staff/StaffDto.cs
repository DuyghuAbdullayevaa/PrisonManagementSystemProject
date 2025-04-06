using PrisonManagementSystem.BL.DTOs.Schedule;
using PrisonManagementSystem.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.DTOs.Staff
{
    public class StaffDto
    {
      
        public DateTime DateOfStarting { get; set; }
        public Guid PrisonId { get; set; }
        public List<ScheduleDto> Schedules { get; set; }
    }
}

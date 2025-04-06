using PrisonManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;

namespace PrisonManagementSystem.DTOs
{
    public class CreateScheduleDto
    {
        public DateTime Date { get; set; }
        public ShiftType ShiftType { get; set; }
        public List<Guid> StaffIds { get; set; }
    }
}

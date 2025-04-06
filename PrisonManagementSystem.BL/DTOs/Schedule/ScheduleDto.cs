using PrisonManagementSystem.DAL.Enums;
using System;

namespace PrisonManagementSystem.BL.DTOs.Schedule
{
    public class ScheduleDto
    {
        public DateTime Date { get; set; }
        public ShiftType ShiftType { get; set; }
    }
}

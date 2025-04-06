using PrisonManagementSystem.DAL.Enums;
using System;

namespace PrisonManagementSystem.DTOs
{
    public class GetScheduleDto
    {
        public Guid Id { get; set; }
        public ShiftType ShiftType { get; set; }
        public DateTime Date { get; set; }
        public Guid StaffId { get; set; }
        public string StaffName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}

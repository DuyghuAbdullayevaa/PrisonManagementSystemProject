using PrisonManagementSystem.BL.DTOs.Schedule;
using PrisonManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;

namespace PrisonManagementSystem.BL.DTOs.Staff
{
    public class CreateStaffDto
    {
        public string Name { get; set; }
        public DateTime DateOfStarting { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public Guid PrisonId { get; set; }
        public PositionType Position { get; set; }
        public List<ScheduleDto> Schedules { get; set; }
    }
}

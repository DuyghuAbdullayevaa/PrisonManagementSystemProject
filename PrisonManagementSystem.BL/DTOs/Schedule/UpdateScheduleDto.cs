using PrisonManagementSystem.DAL.Enums;

namespace PrisonManagementSystem.DTOs
{
    public class UpdateScheduleDto
    {
        public ShiftType ShiftType { get; set; }
        public DateTime Date { get; set; }
    }
}

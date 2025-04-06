using System;

namespace PrisonManagementSystem.BL.DTOs.Staff
{
    public class UpdateStaffDto
    {
        public string Name { get; set; }
        public DateTime DateOfStarting { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public Guid PrisonId { get; set; }
        public string UserId { get; set; }
    }
}

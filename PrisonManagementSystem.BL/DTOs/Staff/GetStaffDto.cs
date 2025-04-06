using PrisonManagementSystem.BL.DTOs.Prison;
using PrisonManagementSystem.BL.DTOs.RequestFeedback;
using PrisonManagementSystem.BL.DTOs.Visit.PrisonManagementSystem.DTOs;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;

namespace PrisonManagementSystem.DTOs
{
    public class GetStaffDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfJoining { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public List<string> Prisons { get; set; }
        public List<GetScheduleDto> Schedules { get; set; }
        public string UserId { get; set; }
    }
}

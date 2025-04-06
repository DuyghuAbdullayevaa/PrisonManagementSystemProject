using PrisonManagementSystem.BL.DTOs.Visit;
using PrisonManagementSystem.BL.DTOs.Visit.PrisonManagementSystem.DTOs;
using PrisonManagementSystem.DAL.Enums;

namespace PrisonManagementSystem.DTOs
{
    public class GetVisitorDto
    {
        public string Name { get; set; }
        public Relationship RelationToPrisoner { get; set; }
        public string PhoneNumber { get; set; }
        public string UserId { get; set; }
        public List<GetVisitDto> Visits { get; set; }
    }
}

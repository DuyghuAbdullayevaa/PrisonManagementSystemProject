using PrisonManagementSystem.DAL.Enums;

namespace PrisonManagementSystem.DTOs
{
    public class UpdateVisitorDto
    {
        public string Name { get; set; } 
        public string UserId { get; set; }
        public string PhoneNumber { get; set; } 
        public Relationship Relationship { get; set; }
    }
}

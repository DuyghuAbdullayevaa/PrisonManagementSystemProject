using PrisonManagementSystem.DAL.Enums;
using System;

namespace PrisonManagementSystem.BL.DTOs.Visit
{
    namespace PrisonManagementSystem.DTOs
    {
        public class UpdateVisitDto
        {
            public DateTime VisitDate { get; set; }
            public int DurationInMinutes { get; set; }
            public VisitType VisitType { get; set; }
            public Guid PrisonerId { get; set; }
            public Guid VisitorId { get; set; }
        }
    }
}

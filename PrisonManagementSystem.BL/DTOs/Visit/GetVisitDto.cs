using PrisonManagementSystem.DAL.Enums;
using System;

namespace PrisonManagementSystem.BL.DTOs.Visit
{
    namespace PrisonManagementSystem.DTOs
    {
        public class GetVisitDto
        {
            public Guid Id { get; set; }
            public Guid VisitorId { get; set; }
            public DateTime VisitDate { get; set; }
            public VisitType VisitType { get; set; }
            public int Duration { get; set; }
            public Guid PrisonerId { get; set; }
            public string PrisonerName { get; set; }
        }
    }
}

using PrisonManagementSystem.DAL.Entities.Base;
using PrisonManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;

namespace PrisonManagementSystem.DAL.Entities.PrisonDBContext
{
    public class Visit : BaseEntity
    {
        public DateTime VisitDate { get; set; }
        public VisitType VisitType { get; set; }
        public int DurationInMinutes { get; set; }
        public Guid PrisonerId { get; set; }
        public Prisoner Prisoner { get; set; }
        public Guid VisitorId { get; set; }
        public Visitor Visitor { get; set; }
    }
}

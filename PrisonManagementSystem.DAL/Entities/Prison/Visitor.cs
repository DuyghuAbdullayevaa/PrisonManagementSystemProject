using PrisonManagementSystem.DAL.Entities.Base;
using PrisonManagementSystem.DAL.Entities.Identity;
using PrisonManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;

namespace PrisonManagementSystem.DAL.Entities.PrisonDBContext
{
    public class Visitor : BaseEntity
    {
        public string Name { get; set; }
        public Relationship RelationToPrisoner { get; set; }
        public string PhoneNumber { get; set; }
        public ICollection<Visit> VisitHistory { get; set; }
        public string? UserId { get; set; }
        public User? User { get; set; }
    }
}

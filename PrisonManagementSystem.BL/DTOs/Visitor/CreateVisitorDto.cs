using PrisonManagementSystem.BL.DTOs.Visit;
using PrisonManagementSystem.BL.DTOs.Visit.PrisonManagementSystem.DTOs;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.DTOs.Visitor
{
    public class CreateVisitorDto
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public Relationship RelationToPrisoner { get; set; }
        public VisitDto Visits { get; set; }
    }
}

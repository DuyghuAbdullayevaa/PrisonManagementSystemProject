using PrisonManagementSystem.DAL.Entities.Base;
using PrisonManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;

namespace PrisonManagementSystem.DAL.Entities.PrisonDBContext
{
    public class Prison : BaseEntity
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public int CurrentInmates { get; set; }
        public bool IsMalePrison { get; set; }
        public PrisonStatus Status { get; set; }
        public ICollection<Cell> Cells { get; set; }
        public ICollection<PrisonStaff> PrisonStaffs { get; set; }
        public ICollection<Incident> Incidents { get; set; }
    }
}

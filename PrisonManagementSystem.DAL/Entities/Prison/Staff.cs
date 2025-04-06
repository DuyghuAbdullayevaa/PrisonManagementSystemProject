using PrisonManagementSystem.DAL.Entities.Base;
using PrisonManagementSystem.DAL.Entities.Identity;
using PrisonManagementSystem.DAL.Entities.Prison;
using PrisonManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;

namespace PrisonManagementSystem.DAL.Entities.PrisonDBContext
{
    public class Staff : BaseEntity
    {
        public string Name { get; set; }
        public DateTime DateOfStarting { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public ICollection<PrisonStaff> PrisonStaffs { get; set; }
        public ICollection<Schedule> Schedules { get; set; }
        public PositionType Position { get; set; }
        public string? UserId { get; set; }
        public User? User { get; set; }
    }
}

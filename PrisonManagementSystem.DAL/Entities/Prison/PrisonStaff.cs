using PrisonManagementSystem.DAL.Entities.Base;
using System;

namespace PrisonManagementSystem.DAL.Entities.PrisonDBContext
{
    public class PrisonStaff : BaseEntity
    {
        public Guid PrisonId { get; set; }
        public Prison Prison { get; set; }

        public Guid StaffId { get; set; }
        public Staff Staff { get; set; }

  
    }
}

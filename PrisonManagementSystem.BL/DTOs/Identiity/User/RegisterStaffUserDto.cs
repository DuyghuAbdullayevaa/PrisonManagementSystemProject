using PrisonManagementSystem.BL.DTOs.Staff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.DTOs.Identiity.User
{
    public class RegisterStaffUserDto
    {
        public CreateUserDto CreateUserDto { get; set; }
        public StaffDto CreateStaffDto { get; set; }

    }
}

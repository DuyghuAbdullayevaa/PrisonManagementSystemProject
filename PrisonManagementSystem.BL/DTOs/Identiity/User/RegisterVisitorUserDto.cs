using PrisonManagementSystem.BL.DTOs.Staff;
using PrisonManagementSystem.BL.DTOs.Visit.PrisonManagementSystem.DTOs;
using PrisonManagementSystem.BL.DTOs.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.DTOs.Identiity.User
{
    public class RegisterVisitorUserDto
    {
        public CreateUserDto CreateUserDto { get; set; }
        public CreateVisitorDto CreateVisitorDto { get; set; }

    }
}


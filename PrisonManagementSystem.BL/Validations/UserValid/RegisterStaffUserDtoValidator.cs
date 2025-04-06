using FluentValidation;
using PrisonManagementSystem.BL.DTOs.Identiity.User;
using PrisonManagementSystem.BL.Validations.StaffValid;
using PrisonManagementSystem.BL.Validators.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.Validations.UserValid
{
    public class RegisterStaffUserDtoValidator : AbstractValidator<RegisterStaffUserDto>
    {
        public RegisterStaffUserDtoValidator()
        {
            // CreateUserDto validation
            RuleFor(x => x.CreateUserDto)
                .NotNull().WithMessage("User information is required.")
                .SetValidator(new CreateUserDtoValidator());

            // StaffDto validation
            RuleFor(x => x.CreateStaffDto)
                .NotNull().WithMessage("Staff information is required.")
                .SetValidator(new StaffValidator());
        }
    }
}

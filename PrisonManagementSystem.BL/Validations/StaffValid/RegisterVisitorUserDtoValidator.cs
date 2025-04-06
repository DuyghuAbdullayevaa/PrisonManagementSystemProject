using FluentValidation;
using PrisonManagementSystem.BL.DTOs.Identiity.User;
using PrisonManagementSystem.BL.Validators;
using PrisonManagementSystem.BL.Validators.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.Validations.StaffValid
{
    public class RegisterVisitorUserDtoValidator : AbstractValidator<RegisterVisitorUserDto>
    {
        public RegisterVisitorUserDtoValidator()
        {
            // CreateUserDto validation
            RuleFor(x => x.CreateUserDto)
                .NotNull().WithMessage("User information is required.")
                .SetValidator(new CreateUserDtoValidator());

            // CreateVisitorDto validation
            RuleFor(x => x.CreateVisitorDto)
                .NotNull().WithMessage("Visitor information is required.")
                .SetValidator(new CreateVisitorDtoValidator());
        }
    }
}

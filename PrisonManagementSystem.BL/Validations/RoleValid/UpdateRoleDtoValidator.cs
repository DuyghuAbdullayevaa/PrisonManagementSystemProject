using FluentValidation;
using PrisonManagementSystem.BL.DTOs.Identiity.Role;

namespace PrisonManagementSystem.BL.Validators.Identiity
{
    public class UpdateRoleDtoValidator : AbstractValidator<UpdateRoleDto>
    {
        public UpdateRoleDtoValidator()
        {
           
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Role name is required.")
                .Length(3, 50).WithMessage("Role name must be between 3 and 50 characters.");
        }
    }
}

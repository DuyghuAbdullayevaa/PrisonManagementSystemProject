using FluentValidation;
using PrisonManagementSystem.BL.DTOs.Identiity.User;
using PrisonManagementSystem.BL.Validators;
using PrisonManagementSystem.BL.Validators.Identity;  


namespace PrisonManagementSystem.BL.Validations.UserValid
{
    public class RegisterVisitorUserDtoValidator : AbstractValidator<RegisterVisitorUserDto>
    {
        public RegisterVisitorUserDtoValidator()
        {
            // Validate CreateUserDto using the CreateUserDtoValidator
            RuleFor(x => x.CreateUserDto)
               .NotNull().WithMessage("User information is required.")
               .SetValidator(new CreateUserDtoValidator());

            // Validate CreateVisitorDto using the correct validator (CreateVisitorDtoValidator)
            RuleFor(x => x.CreateVisitorDto)
                .NotNull().WithMessage("Visitor information is required.")
                .SetValidator(new CreateVisitorDtoValidator()); // Fix: Use CreateVisitorDtoValidator
        }
    }
}

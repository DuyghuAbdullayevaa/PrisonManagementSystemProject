using FluentValidation;
using PrisonManagementSystem.DTOs;
using System.Text.RegularExpressions;

namespace PrisonManagementSystem.BL.Validators
{
    public class UpdateVisitorDtoValidator : AbstractValidator<UpdateVisitorDto>
    {
        public UpdateVisitorDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Visitor's name is required.")
                .MaximumLength(100).WithMessage("Visitor's name can be a maximum of 100 characters.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?\d{9,15}$").WithMessage("Phone number is in an invalid format.");

            RuleFor(x => x.Relationship)
                .IsInEnum().WithMessage("A valid relationship type must be provided.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required.");
        }
    }
}

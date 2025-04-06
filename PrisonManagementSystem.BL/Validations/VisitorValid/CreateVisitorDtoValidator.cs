using FluentValidation;
using PrisonManagementSystem.BL.DTOs.Visitor;
using System.Text.RegularExpressions;

namespace PrisonManagementSystem.BL.Validators
{
    public class CreateVisitorDtoValidator : AbstractValidator<CreateVisitorDto>
    {
        public CreateVisitorDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Visitor's name is required.")
                .MaximumLength(100).WithMessage("Visitor's name can be a maximum of 100 characters.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?\d{9,15}$").WithMessage("Phone number is in an invalid format.");

            RuleFor(x => x.RelationToPrisoner)
                .IsInEnum().WithMessage("A valid relationship type must be provided.");

            RuleFor(x => x.Visits)
                .NotNull().WithMessage("The visitor must have at least one visit.")
                .SetValidator(new VisitDtoValidator());
        }
    }
}

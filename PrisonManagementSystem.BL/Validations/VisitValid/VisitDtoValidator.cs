using FluentValidation;
using PrisonManagementSystem.BL.DTOs.Visit;
using System;

namespace PrisonManagementSystem.BL.Validators
{
    public class VisitDtoValidator : AbstractValidator<VisitDto>
    {
        public VisitDtoValidator()
        {
            RuleFor(x => x.VisitDate)
                .NotEmpty().WithMessage("Visit date is required.")
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Visit date must be today or in the future.");

            RuleFor(x => x.DurationInMinutes)
                .GreaterThan(0).WithMessage("Visit duration must be greater than 0.");

            RuleFor(x => x.VisitType)
                .IsInEnum().WithMessage("A valid visit type must be provided.");

            RuleFor(x => x.PrisonerId)
                .NotEmpty().WithMessage("Prisoner ID is required.");
        }
    }
}

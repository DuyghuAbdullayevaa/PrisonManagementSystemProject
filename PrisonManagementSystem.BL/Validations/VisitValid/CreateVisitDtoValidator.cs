using FluentValidation;
using PrisonManagementSystem.BL.DTOs.Visit.PrisonManagementSystem.DTOs;
using PrisonManagementSystem.DTOs;
using System;

namespace PrisonManagementSystem.BL.Validators
{
    public class CreateVisitDtoValidator : AbstractValidator<CreateVisitDto>
    {
        public CreateVisitDtoValidator()
        {
            RuleFor(x => x.VisitDate)
                .NotEmpty().WithMessage("Visit date is required.")
                .GreaterThan(DateTime.UtcNow).WithMessage("Visit date cannot be in the past.");

            RuleFor(x => x.VisitType)
                .IsInEnum().WithMessage("A valid visit type must be provided.");

            RuleFor(x => x.DurationInMinutes)
                .GreaterThan(0).WithMessage("Visit duration must be positive.")
                .LessThanOrEqualTo(180).WithMessage("Visit duration can be a maximum of 180 minutes.");

            RuleFor(x => x.PrisonerId)
                .NotEmpty().WithMessage("Prisoner ID is required.");

            RuleFor(x => x.VisitorId)
                .NotEmpty().WithMessage("Visitor ID is required.");
        }
    }
}

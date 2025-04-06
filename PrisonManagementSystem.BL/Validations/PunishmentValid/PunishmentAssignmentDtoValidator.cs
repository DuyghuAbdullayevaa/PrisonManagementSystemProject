using FluentValidation;
using PrisonManagementSystem.BL.DTOs.Punishment;
using System;

namespace PrisonManagementSystem.BL.Validators
{
    public class PunishmentAssignmentDtoValidator : AbstractValidator<PunishmentAssignmentDto>
    {
        public PunishmentAssignmentDtoValidator()
        {
            // Validate PrisonerId
            RuleFor(x => x.PrisonerId)
                .NotEmpty().WithMessage("Prisoner ID is required.");

            // Validate PunishmentType
            RuleFor(x => x.PunishmentType)
                .IsInEnum().WithMessage("Punishment type must be valid.");

            // Validate StartDate
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Start date cannot be in the future.");

            // Validate EndDate
            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate).WithMessage("End date must be later than start date.")
                .When(x => x.EndDate.HasValue); // Only validate if EndDate is provided
        }
    }
}

using FluentValidation;
using PrisonManagementSystem.BL.DTOs.Punishment;
using PrisonManagementSystem.DTOs;
using System;

namespace PrisonManagementSystem.BL.Validators
{
    public class CreatePunishmentDtoValidator : AbstractValidator<CreatePunishmentDto>
    {
        public CreatePunishmentDtoValidator()
        {
            RuleFor(x => x.IncidentId)
                .NotEmpty().WithMessage("Incident ID is required.");

            RuleFor(x => x.Punishments)
                .NotEmpty().WithMessage("Punishments list cannot be empty.")
                .Must(p => p != null && p.Count > 0).WithMessage("At least one punishment must be assigned to a prisoner.");

            RuleForEach(x => x.Punishments)
                .SetValidator(new PunishmentAssignmentDtoValidator());
        }
    }
}
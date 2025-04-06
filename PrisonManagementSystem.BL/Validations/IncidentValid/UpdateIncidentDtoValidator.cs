using FluentValidation;
using PrisonManagementSystem.BL.DTOs.Incident;
using PrisonManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;

namespace PrisonManagementSystem.BL.Validators
{
    public class UpdateIncidentDtoValidator : AbstractValidator<UpdateIncidentDto>
    {
        public UpdateIncidentDtoValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(1000).WithMessage("Description can be up to 1000 characters long.");

            RuleFor(x => x.IncidentDate)
                .NotEmpty().WithMessage("Incident date is required.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Incident date cannot be in the future.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid incident status.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Invalid incident type.");

            RuleFor(x => x.PrisonerIds)
                .Must(x => x == null || x.Count > 0).WithMessage("At least one prisoner ID must be provided if the list is not empty.");

            RuleFor(x => x.CellIds)
                .Must(x => x == null || x.Count > 0).WithMessage("At least one cell ID must be provided if the list is not empty.");
        }
    }
}

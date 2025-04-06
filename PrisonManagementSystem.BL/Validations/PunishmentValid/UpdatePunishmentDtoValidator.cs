using FluentValidation;
using PrisonManagementSystem.DTOs;

namespace PrisonManagementSystem.BL.Validators
{
    public class UpdatePunishmentDtoValidator : AbstractValidator<UpdatePunishmentDto>
    {
        public UpdatePunishmentDtoValidator()
        {
            RuleFor(x => x.PrisonerId)
                .NotEmpty().WithMessage("Prisoner ID is required.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Start date cannot be in the future.");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate).WithMessage("End date must be after the start date.")
                .When(x => x.EndDate.HasValue); 

            RuleFor(x => x.IncidentId)
                .NotEmpty().WithMessage("Incident ID is required.");

            RuleFor(x => x.PunishmentType)
                .IsInEnum().WithMessage("Punishment type must be a valid value.");
        }
    }
}

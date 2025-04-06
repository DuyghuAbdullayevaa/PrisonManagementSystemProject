using FluentValidation;
using PrisonManagementSystem.BL.DTOs.Incident;

namespace PrisonManagementSystem.BL.Validators
{
    public class CreateIncidentDtoValidator : AbstractValidator<CreateIncidentDto>
    {
        public CreateIncidentDtoValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Incident description is required.")
                .MaximumLength(500).WithMessage("Incident description cannot exceed 500 characters.");

            RuleFor(x => x.IncidentDate)
                .NotEmpty().WithMessage("Incident date is required.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Incident date cannot be in the future.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Incident status must be a valid value.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Incident type must be a valid value.");

            RuleFor(x => x.PrisonId)
                .NotEmpty().WithMessage("Prison ID is required.");

            RuleFor(x => x.Report)
                .NotNull().WithMessage("Report is required.")
                .SetValidator(new CreateReportDtoValidator()); // Validate the embedded Report DTO
        }
    }
}

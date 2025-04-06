using FluentValidation;
using PrisonManagementSystem.BL.DTOs.Prisoner;

namespace PrisonManagementSystem.BL.Validators
{
    public class UpdatePrisonerDtoValidator : AbstractValidator<UpdatePrisonerDto>
    {
        public UpdatePrisonerDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.")
                .LessThan(DateTime.Now).WithMessage("Date of birth cannot be in the future.");

            RuleFor(x => x.AdmissionDate)
                .NotEmpty().WithMessage("Admission date is required.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Admission date cannot be in the future.");

            RuleFor(x => x.ReleaseDate)
                .GreaterThanOrEqualTo(x => x.AdmissionDate).WithMessage("Release date cannot be earlier than admission date.")
                .When(x => x.ReleaseDate.HasValue); // Only validate if ReleaseDate is provided

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Prisoner status must be a valid value.");

            RuleFor(x => x.CellId)
                .NotEmpty().WithMessage("Cell ID is required.");
        }
    }
}

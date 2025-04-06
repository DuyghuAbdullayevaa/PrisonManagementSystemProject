using FluentValidation;
using PrisonManagementSystem.BL.DTOs.Prisoner;
using PrisonManagementSystem.BL.DTOs.Crime;

namespace PrisonManagementSystem.BL.Validators
{
    public class CreatePrisonerDtoValidator : AbstractValidator<CreatePrisonerDto>
    {
        public CreatePrisonerDtoValidator()
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
                .NotEmpty().WithMessage("Admission date is required.");

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Gender must be a valid value.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Prisoner status must be a valid value.");

            RuleFor(x => x.CellId)
                .NotEmpty().WithMessage("Cell ID is required.");

            RuleFor(x => x.Crimes)
                .NotNull().WithMessage("Crimes list cannot be null.")
                .NotEmpty().WithMessage("At least one crime must be associated with the prisoner.")
                .ForEach(crime => crime.SetValidator(new CrimeDtoValidator()));
        }
    }
}

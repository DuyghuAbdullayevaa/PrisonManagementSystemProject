using FluentValidation;
using PrisonManagementSystem.BL.DTOs.Crime;

namespace PrisonManagementSystem.BL.Validators
{
    public class CrimeDtoValidator : AbstractValidator<CrimeDto>
    {
        public CrimeDtoValidator()
        {
            RuleFor(x => x.Details)
                .NotEmpty().WithMessage("Crime details are required.")
                .MaximumLength(500).WithMessage("Crime details cannot exceed 500 characters.");

            RuleFor(x => x.SeverityLevel)
                .IsInEnum().WithMessage("Crime severity must be a valid value.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Crime type must be a valid value.");
        }
    }
}

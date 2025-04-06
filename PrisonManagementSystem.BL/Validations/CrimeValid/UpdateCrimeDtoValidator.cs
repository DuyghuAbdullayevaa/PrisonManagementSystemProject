using FluentValidation;
using PrisonManagementSystem.BL.DTOs.Crime;
using PrisonManagementSystem.DAL.Enums;

public class UpdateCrimeDtoValidator : AbstractValidator<UpdateCrimeDto>
{
    public UpdateCrimeDtoValidator()
    {
        RuleFor(x => x.Details)
            .NotEmpty().WithMessage("Crime details cannot be empty.")
            .MaximumLength(500).WithMessage("Crime details can be up to 500 characters long.");

        RuleFor(x => x.SeverityLevel)
            .IsInEnum().WithMessage("A valid crime severity level must be selected.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("A valid crime type must be selected.");

        RuleFor(x => x.MinimumSentence)
            .GreaterThanOrEqualTo(0).WithMessage("The minimum sentence cannot be negative.");

        RuleFor(x => x.MaximumSentence)
            .GreaterThan(x => x.MinimumSentence).WithMessage("The maximum sentence must be greater than the minimum sentence.")
            .GreaterThanOrEqualTo(0).WithMessage("The maximum sentence cannot be negative.");
    }
}

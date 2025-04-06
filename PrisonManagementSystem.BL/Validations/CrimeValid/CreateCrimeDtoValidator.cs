using FluentValidation;
using PrisonManagementSystem.BL.DTOs.Crime;
using PrisonManagementSystem.BL.DTOs.Prisoner;

public class CreateCrimeDtoValidator : AbstractValidator<CreateCrimeDto>
{
    public CreateCrimeDtoValidator()
    {
        RuleFor(x => x.Details)
                 .NotEmpty().WithMessage("Crime details are required.")
                 .Length(10, 500).WithMessage("Crime details must be between 10 and 500 characters.");

        RuleFor(x => x.SeverityLevel)
            .IsInEnum().WithMessage("Invalid crime severity level.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid crime type.");

        RuleFor(x => x.Prisoners)
            .NotEmpty().WithMessage("At least one prisoner must be associated with the crime.")
            .Must(prisoners => prisoners.Count > 0).WithMessage("Prisoners list cannot be empty.");
        RuleForEach(x => x.Prisoners)
          .SetValidator(new PrisonerDtoValidator());
    }
}

using FluentValidation;
using PrisonManagementSystem.BL.DTOs.Prison;

public class UpdatePrisonDtoValidator : AbstractValidator<UpdatePrisonDto>
{
    public UpdatePrisonDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Prison name cannot be empty.")
            .MaximumLength(100).WithMessage("Prison name can be a maximum of 100 characters.");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Prison location cannot be empty.")
            .MaximumLength(200).WithMessage("Prison location can be a maximum of 200 characters.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Prison capacity must be a positive value.");

        RuleFor(x => x.IsMalePrison)
            .NotNull().WithMessage("It must be selected whether the prison is a male prison or not.");

        RuleFor(x => x.IsActive)
            .NotNull().WithMessage("It must be selected whether the prison is active or not.");
    }
}

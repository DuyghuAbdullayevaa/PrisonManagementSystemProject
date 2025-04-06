using FluentValidation;
using PrisonManagementSystem.BL.DTOs.Cell;

public class CellCreateDtoValidator : AbstractValidator<CreateCellDto>
{
    public CellCreateDtoValidator()
    {
        RuleFor(x => x.CellNumber)
            .NotEmpty().WithMessage("Cell number is required.")
            .Length(1, 20).WithMessage("Cell number must be between 1 and 20 characters.");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than zero.");

        RuleFor(x => x.PrisonId)
            .NotEmpty().WithMessage("Prison Id is required.")
            .NotEqual(Guid.Empty).WithMessage("Prison Id cannot be empty.");
    }
}

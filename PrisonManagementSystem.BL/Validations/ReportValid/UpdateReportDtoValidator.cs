using FluentValidation;
using PrisonManagementSystem.DTOs;

public class UpdateReportDtoValidator : AbstractValidator<UpdateReportDto>
{
    public UpdateReportDtoValidator()
    {

        RuleFor(x => x.Descriptions)
                .NotEmpty().WithMessage("Report description cannot be empty.")
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.ReportType)
            .IsInEnum().WithMessage("Report type must be a valid enum value.");
    }
}

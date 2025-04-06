using FluentValidation;
using PrisonManagementSystem.DTOs;

namespace PrisonManagementSystem.BL.Validators
{
    public class CreateReportDtoValidator : AbstractValidator<CreateReportDto>
    {
        public CreateReportDtoValidator()
        {
            RuleFor(x => x.Descriptions)
                 .NotEmpty().WithMessage("Report description cannot be empty.")
                 .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

            RuleFor(x => x.ReportType)
                .IsInEnum().WithMessage("Report type must be a valid enum value.");
        }
    }
}

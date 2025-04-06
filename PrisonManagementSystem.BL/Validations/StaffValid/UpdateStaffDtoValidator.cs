using FluentValidation;
using PrisonManagementSystem.BL.DTOs.Staff;

public class UpdateStaffDtoValidator : AbstractValidator<UpdateStaffDto>
{
    public UpdateStaffDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.DateOfStarting)
            .NotEmpty().WithMessage("Start date cannot be empty.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Start date cannot be in the future.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number cannot be empty.")
            .Matches(@"^\+?\d{10,15}$").WithMessage("Phone number must be in a valid format.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email cannot be empty.")
            .EmailAddress().WithMessage("Please enter a valid email address.");

        RuleFor(x => x.PrisonId)
            .NotEmpty().WithMessage("Prison ID cannot be empty.");
    }
}
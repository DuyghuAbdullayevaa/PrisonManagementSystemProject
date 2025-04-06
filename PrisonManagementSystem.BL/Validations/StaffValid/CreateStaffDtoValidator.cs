using FluentValidation;
using PrisonManagementSystem.BL.DTOs.Staff;
using PrisonManagementSystem.BL.Validations.ScheduleValid;
using System;
using System.Text.RegularExpressions;

namespace PrisonManagementSystem.BL.Validators
{
    public class CreateStaffDtoValidator : AbstractValidator<CreateStaffDto>
    {
        public CreateStaffDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name can be a maximum of 100 characters.");

            RuleFor(x => x.DateOfStarting)
                .NotEmpty().WithMessage("Start date is required.")
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Start date must be today or a future date.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?\d{9,15}$").WithMessage("Phone number is in an invalid format.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address must be provided.");

            RuleFor(x => x.PrisonId)
                .NotEmpty().WithMessage("Prison ID is required.");

            RuleFor(x => x.Position)
               .IsInEnum().WithMessage("A valid position (PositionType) must be selected.");

            RuleFor(x => x.Schedules)
                .NotEmpty().WithMessage("The staff must have at least one schedule.")
                .ForEach(schedule =>
                {
                    schedule.NotNull().WithMessage("Schedule information cannot be empty.");
                    schedule.SetValidator(new ScheduleDtoValidator());
                });
        }
    }
}

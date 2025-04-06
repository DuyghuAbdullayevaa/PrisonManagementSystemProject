using FluentValidation;
using PrisonManagementSystem.DTOs;
using System;

namespace PrisonManagementSystem.BL.Validators
{
    public class UpdateScheduleDtoValidator : AbstractValidator<UpdateScheduleDto>
    {
        public UpdateScheduleDtoValidator()
        {
            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Schedule date is required.")
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Schedule date must be today or a future date.");

            RuleFor(x => x.ShiftType)
                .IsInEnum().WithMessage("A valid shift type must be provided.");
        }
    }
}

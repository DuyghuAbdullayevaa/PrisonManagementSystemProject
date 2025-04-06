using FluentValidation;
using PrisonManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;

namespace PrisonManagementSystem.DTOs
{

    public class CreateScheduleDtoValidator : AbstractValidator<CreateScheduleDto>
    {
        public CreateScheduleDtoValidator()
        {
            RuleFor(x => x.Date)
                .GreaterThanOrEqualTo(DateTime.Now).WithMessage("Shift date cannot be in the past.");

            RuleFor(x => x.ShiftType)
                .IsInEnum().WithMessage("Please provide a valid shift type.");

            RuleFor(x => x.StaffIds)
                .NotEmpty().WithMessage("Staff IDs cannot be empty.")
                .Must(staffIds => staffIds.All(id => id != Guid.Empty))
                .WithMessage("Each staff ID must be a valid GUID.");
        }
    }
}

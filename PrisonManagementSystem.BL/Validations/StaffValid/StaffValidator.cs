using FluentValidation;
using PrisonManagementSystem.BL.DTOs.Staff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.Validations.StaffValid
{
    public class StaffValidator : AbstractValidator<StaffDto>
    {
        public StaffValidator()
        {
            RuleFor(x => x.DateOfStarting)
                .NotEmpty().WithMessage("Date of starting is required.")
                .LessThan(DateTime.Now).WithMessage("Date of starting must be in the past.");

            RuleFor(x => x.PrisonId)
                .NotEmpty().WithMessage("Prison ID is required.")
                .NotEqual(Guid.Empty).WithMessage("Prison ID cannot be empty.");

            RuleFor(x => x.Schedules)
                .NotEmpty().WithMessage("Schedules are required.")
                .Must(s => s.Count > 0).WithMessage("At least one schedule must be provided.");
        }
    }
}

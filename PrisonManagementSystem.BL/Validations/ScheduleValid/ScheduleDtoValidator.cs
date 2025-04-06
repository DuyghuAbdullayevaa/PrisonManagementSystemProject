using FluentValidation;
using PrisonManagementSystem.BL.DTOs.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.Validations.ScheduleValid
{
    public class ScheduleDtoValidator : AbstractValidator<ScheduleDto>
    {
        public ScheduleDtoValidator()
        {
            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Shift date is required.");
            

            RuleFor(x => x.ShiftType)
                .IsInEnum().WithMessage("Invalid shift type.");
        }
    }
}

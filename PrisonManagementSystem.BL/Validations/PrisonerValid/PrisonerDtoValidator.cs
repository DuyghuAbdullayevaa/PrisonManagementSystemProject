using FluentValidation;
using PrisonManagementSystem.DAL.Enums;
using System;

namespace PrisonManagementSystem.BL.DTOs.Prisoner
{
    public class PrisonerDtoValidator : AbstractValidator<PrisonerDto>
    {
        public PrisonerDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .Length(2, 100).WithMessage("First name must be between 2 and 100 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .Length(2, 100).WithMessage("Last name must be between 2 and 100 characters.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.")
                .LessThan(DateTime.Now).WithMessage("Date of birth must be a past date.");

            RuleFor(x => x.AdmissionDate)
                .NotEmpty().WithMessage("Admission date is required.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Admission date cannot be in the future.");

            RuleFor(x => x.ReleaseDate)
                .GreaterThanOrEqualTo(x => x.AdmissionDate).When(x => x.ReleaseDate.HasValue)
                .WithMessage("Release date must be after admission date.");

            RuleFor(x => x.HasPreviousConvictions)
                .NotNull().WithMessage("Previous convictions status is required.");

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Invalid gender.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid prisoner status.");

            

        }
    }
}

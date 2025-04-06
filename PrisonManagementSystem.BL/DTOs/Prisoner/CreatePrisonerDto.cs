using PrisonManagementSystem.BL.DTOs.Crime;
using PrisonManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;

namespace PrisonManagementSystem.BL.DTOs.Prisoner
{
    public class CreatePrisonerDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime AdmissionDate { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public bool HasPreviousConvictions { get; set; }
        public PrisonerStatus Status { get; set; }
        public Guid CellId { get; set; }
        public List<CrimeDto> Crimes { get; set; }
    }
}

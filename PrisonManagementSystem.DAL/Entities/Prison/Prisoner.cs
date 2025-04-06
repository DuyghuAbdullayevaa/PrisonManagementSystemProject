using PrisonManagementSystem.DAL.Entities.Base;
using PrisonManagementSystem.DAL.Entities.Identity;
using PrisonManagementSystem.DAL.Entities.Prison;
using PrisonManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;

namespace PrisonManagementSystem.DAL.Entities.PrisonDBContext
{
    public class Prisoner : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public bool HasPreviousConvictions { get; set; }
        public DateTime AdmissionDate { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public PrisonerStatus Status { get; set; }
        public ICollection<Visit> Visits { get; set; }
        public ICollection<PrisonerCrime> PrisonerCrimes { get; set; }
        public Guid CellId { get; set; }
        public Cell Cell { get; set; }
        public ICollection<PrisonerPunishment>? PrisonerPunishments { get; set; }
        public ICollection<PrisonerIncident> PrisonersIncidents { get; set; }
     
    }
}

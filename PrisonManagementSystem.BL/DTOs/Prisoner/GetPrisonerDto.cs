using PrisonManagementSystem.BL.DTOs.Crime;
using PrisonManagementSystem.BL.DTOs.Incident;
using PrisonManagementSystem.BL.DTOs.Punishment;
using PrisonManagementSystem.BL.DTOs.RequestFeedback;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.DTOs.Prisoner
{
    public class GetPrisonerDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public bool HasPreviousConvictions { get; set; }
        public DateTime AdmissionDate { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public PrisonerStatus Status { get; set; }
        public string CellNumber { get; set; }
        public List<GetCrimeDto> Crimes { get; set; }
        public List<PunishmentDto> Punishments { get; set; }
        public List<IncidentDto> Incidents { get; set; }
    }
}

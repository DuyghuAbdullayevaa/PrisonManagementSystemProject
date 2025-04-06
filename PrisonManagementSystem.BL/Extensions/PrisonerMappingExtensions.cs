using PrisonManagementSystem.BL.DTOs.Crime;
using PrisonManagementSystem.BL.DTOs.Incident;
using PrisonManagementSystem.BL.DTOs.Prisoner;
using PrisonManagementSystem.BL.DTOs.Punishment;
using PrisonManagementSystem.BL.DTOs.RequestFeedback;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.Extensions
{
    public static class PrisonerMappingExtensions
    {
        public static GetPrisonerDto ToGetPrisonerDto(this Prisoner prisoner)
        {
            if (prisoner == null) return null;

            return new GetPrisonerDto
            {
                Id = prisoner.Id,
                FirstName = prisoner.FirstName,
                LastName = prisoner.LastName,
                MiddleName = prisoner.MiddleName,
                DateOfBirth = prisoner.DateOfBirth,
                Gender = prisoner.Gender,
                HasPreviousConvictions = prisoner.HasPreviousConvictions,
                AdmissionDate = prisoner.AdmissionDate,
                ReleaseDate = prisoner.ReleaseDate,
                Status = prisoner.Status,
                CellNumber = prisoner.Cell?.CellNumber,
                Crimes = prisoner.MapCrimes(),
                Punishments = prisoner.MapPunishments(),
                Incidents = prisoner.MapIncidents()
            };
        }

        private static List<GetCrimeDto> MapCrimes(this Prisoner prisoner)
        {
            return prisoner.PrisonerCrimes?.Select(pc => new GetCrimeDto
            {
                Id = pc.Crime.Id,
                Details = pc.Crime.Details,
                SeverityLevel = pc.Crime.SeverityLevel,
                Type = pc.Crime.Type,
                Prisoners = new List<string> { $"{prisoner.FirstName} {prisoner.LastName}" }
            }).ToList();
        }

        private static List<PunishmentDto> MapPunishments(this Prisoner prisoner)
        {
            return prisoner.PrisonerPunishments?.Select(pp => new PunishmentDto
            {
                Id = pp.Punishment.Id,
                Type = pp.Punishment.Type,
                StartDate = pp.Punishment.StartDate,
                EndDate = pp.Punishment.EndDate
            }).ToList();
        }

        private static List<IncidentDto> MapIncidents(this Prisoner prisoner)
        {
            return prisoner.PrisonersIncidents?.Select(pi => pi.Incident.ToDto()).ToList();
        }

        public static IncidentDto ToDto(this Incident incident)
        {
            if (incident == null) return null;

            return new IncidentDto
            {
                Id = incident.Id,
                Description = incident.Description,
                IncidentDate = incident.IncidentDate,
                Status = incident.Status,
                Type = incident.Type,
               
          
            };
        }
    }
}

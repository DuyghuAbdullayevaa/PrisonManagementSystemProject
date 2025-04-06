using PrisonManagementSystem.BL.DTOs.Cell;
using PrisonManagementSystem.BL.DTOs.Prisoner;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.Extensions
{
    public static class CellMappingExtensions
    {
        public static GetCellDto ToGetCellDto(this Cell cell)
        {
            return new GetCellDto
            {
                Id = cell.Id,
                CellNumber = cell.CellNumber,
                Capacity = cell.Capacity,
                CurrentOccupancy = cell.CurrentOccupancy,
                Status = cell.Status,
                PrisonName = cell.Prison?.Name ?? "Unknown", 
                Prisoners = cell.Prisoners?.Select(prisoner => new PrisonerDto
                {
                    FirstName = prisoner.FirstName,
                    LastName = prisoner.LastName,
                    MiddleName = prisoner.MiddleName,
                    AdmissionDate = prisoner.AdmissionDate,
                    ReleaseDate = prisoner.ReleaseDate,
                    DateOfBirth = prisoner.DateOfBirth,
                    Gender = prisoner.Gender,
                    HasPreviousConvictions = prisoner.HasPreviousConvictions,
                    Status = prisoner.Status,
                }).ToList() ?? new List<PrisonerDto>() 
            };
        }
    }

}

using PrisonManagementSystem.BL.DTOs.Cell;
using PrisonManagementSystem.BL.DTOs.Incident;
using PrisonManagementSystem.DAL.Enums;
using PrisonManagementSystem.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.DTOs.Prison
{
    public class GetPrisonDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public int CurrentInmates { get; set; }
        public bool IsMalePrison { get; set; }
        public bool IsActive { get; set; }
        public ICollection<GetCellDto> Cells { get; set; }
        public ICollection<GetStaffDto> Staffs { get; set; }
        public ICollection<GetIncidentDto> Incidents { get; set; }
    }
}

using PrisonManagementSystem.BL.DTOs.Incident;
using PrisonManagementSystem.BL.DTOs.Prisoner;
using PrisonManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.DTOs.Cell
{
    public class GetCellDto
    {
        public Guid Id { get; set; } 
        public string CellNumber { get; set; } 
        public int Capacity { get; set; } 
        public int CurrentOccupancy { get; set; } 
        public CellStatus Status { get; set; } 
        public string PrisonName { get; set; } 
        public ICollection<PrisonerDto> Prisoners { get; set; }
     
    }
}

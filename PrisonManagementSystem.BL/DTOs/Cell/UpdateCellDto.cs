using PrisonManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.DTOs.Cell
{
    public class UpdateCellDto
    {
        public string CellNumber { get; set; } 
        public int Capacity { get; set; } 
        public CellStatus? Status { get; set; } 
        public Guid PrisonId { get; set; } 
    }

}

using PrisonManagementSystem.DAL.Entities.Base;
using PrisonManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;

namespace PrisonManagementSystem.DAL.Entities.PrisonDBContext
{
    public class Cell : BaseEntity
    {
        public string CellNumber { get; set; } // Cell number
        public int Capacity { get; set; } // Cell capacity
        public int CurrentOccupancy { get; set; } // Current number of prisoners
        public CellStatus Status { get; set; } // Cell status (Full, Available)
        public Guid PrisonId { get; set; } // Reference to the prison
        public Prison Prison { get; set; }
        public ICollection<IncidentCell>? IncidentCells { get; set; }
        public ICollection<Prisoner> Prisoners { get; set; } // Prisoners in the cell
    }
}

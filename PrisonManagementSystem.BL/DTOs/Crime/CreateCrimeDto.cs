using PrisonManagementSystem.BL.DTOs.Prisoner;
using PrisonManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.DTOs.Crime
{
    public class CreateCrimeDto
    {
        public string Details { get; set; }
        public CrimeSeverity SeverityLevel { get; set; }
        public CrimeType Type { get; set; }
        public List<PrisonerDto> Prisoners { get; set; }
    }

}

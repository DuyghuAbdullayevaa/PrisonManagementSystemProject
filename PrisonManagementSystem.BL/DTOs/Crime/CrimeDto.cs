using PrisonManagementSystem.DAL.Enums;

namespace PrisonManagementSystem.BL.DTOs.Crime
{
    public class CrimeDto
    {
        public string Details { get; set; }
        public CrimeSeverity SeverityLevel { get; set; } 
        public CrimeType Type { get; set; } 
    }
}

using PrisonManagementSystem.DAL.Enums;

namespace PrisonManagementSystem.DTOs
{
    public class CreateReportDto
    { 
       
        public string Descriptions { get; set; } 
        public ReportType ReportType { get; set; }  
    }
}

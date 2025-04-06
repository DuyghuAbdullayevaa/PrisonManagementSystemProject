using PrisonManagementSystem.DAL.Enums;

namespace PrisonManagementSystem.DTOs
{
    public class UpdateReportDto
    {
        public string Descriptions { get; set; }
        public ReportType ReportType { get; set; }
    }
}

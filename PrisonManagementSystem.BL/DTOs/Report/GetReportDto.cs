using PrisonManagementSystem.BL.DTOs.Incident;
using PrisonManagementSystem.DAL.Enums;
using System;

namespace PrisonManagementSystem.BL.DTOs.RequestFeedback
{
    public class GetReportDto
    {
        public Guid Id { get; set; }
        public ReportType ReportType { get; set; }
        public IncidentDto Incident { get; set; }
        public string Descriptions { get; set; }
    }
}

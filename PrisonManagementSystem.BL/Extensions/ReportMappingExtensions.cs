using PrisonManagementSystem.BL.DTOs.Incident;
using PrisonManagementSystem.BL.DTOs.RequestFeedback;
using PrisonManagementSystem.DAL.Entities.Prison;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.Extensions
{
    public static class ReportMappingExtensions
    {
        public static GetReportDto ToGetReportDto(this Report report)
        {
            return new GetReportDto
            {
                Id = report.Id,
                ReportType = report.ReportType,
                Descriptions = report.Descriptions,
                Incident = report.RelatedIncident != null ? new IncidentDto
                {
                    Id = report.RelatedIncident.Id,
                    Description = report.RelatedIncident.Description,
                    IncidentDate = report.RelatedIncident.IncidentDate,
                    Status = report.RelatedIncident.Status,
                    Type = report.RelatedIncident.Type
                } : null
            };
        }
    }
}

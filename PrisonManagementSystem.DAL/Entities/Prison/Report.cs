using PrisonManagementSystem.DAL.Entities.Base;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;

namespace PrisonManagementSystem.DAL.Entities.Prison
{
    public class Report : BaseEntity
    {
        public string Descriptions { get; set; }
        public ReportType ReportType { get; set; }
        public Guid? RelatedIncidentId { get; set; }
        public Incident RelatedIncident { get; set; }
    }
}

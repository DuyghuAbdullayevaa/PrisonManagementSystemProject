using System;

namespace PrisonManagementSystem.DAL.Enums
{
    public enum ReportType
    {
        Incident,           // General report about an incident
        Investigation,      // Report related to the investigation of the incident
        WitnessStatement,   // Statements from witnesses
        OfficerReport       // Report by a guard or staff member
    }
}

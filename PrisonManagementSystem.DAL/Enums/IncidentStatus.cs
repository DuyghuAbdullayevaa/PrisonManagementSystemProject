using System;

namespace PrisonManagementSystem.DAL.Enums
{
    public enum IncidentStatus
    {
        PendingInvestigation, // Investigation pending
        UnderInvestigation,   // Under investigation
        Resolved,             // Resolved
        Escalated,            // Escalated to a higher level
        Dismissed             // Dismissed
    }
}

using System;

namespace PrisonManagementSystem.DAL.Enums
{
    public enum PunishmentType
    {
        SolitaryConfinement,     // Placement in solitary confinement
        NoVisitation,            // Visitation rights revoked
        ReducedPrivileges,       // Reduction of personal privileges
        ExtendedSentence,        // Extension of prison sentence
        IncreasedSecurityLevel   // Elevation to a higher security level
    }
}

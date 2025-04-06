using System;

namespace PrisonManagementSystem.DAL.Enums
{
    public enum Relationship
    {
        // Personal relationships
        Parent,         // Parent of the prisoner
        Sibling,        // Brother or sister of the prisoner
        Spouse,         // Spouse of the prisoner
        Child,          // Child of the prisoner
        Friend,         // Friend of the prisoner

        // Professional relationships (may have system access)
        Lawyer,         // Legal representative
        Official,       // Government or official authority
        SocialWorker    // Assigned social worker
    }
}

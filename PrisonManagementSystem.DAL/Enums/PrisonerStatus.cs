using System;

namespace PrisonManagementSystem.DAL.Enums
{
    public enum PrisonerStatus
    {
        Active,         // Currently incarcerated
        Released,       // Released from prison
        Deceased,       // Deceased while in custody
        Transferred,    // Transferred to another facility
        Reoffended,     // Returned due to a new offense
        LifeSentence    // Serving a life sentence
    }
}

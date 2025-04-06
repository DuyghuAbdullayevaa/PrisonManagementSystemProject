using PrisonManagementSystem.DAL.Entities.Base;
using PrisonManagementSystem.DAL.Enums;
using System;
using System.Collections.Generic;

namespace PrisonManagementSystem.DAL.Entities.PrisonDBContext
{
    public class Crime : BaseEntity
    {
        public string Details { get; set; }
        public CrimeSeverity SeverityLevel { get; set; }
        public CrimeType Type { get; set; }
        public int MinimumSentence { get; set; }
        public int MaximumSentence { get; set; }
        public ICollection<PrisonerCrime> PrisonerCrimes { get; set; }
    }
}

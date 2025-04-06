using Microsoft.AspNetCore.Identity;
using PrisonManagementSystem.DAL.Entities.Base;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Entities.Identity
{
    public class User : IdentityUser<string>
    { 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        public DateTime BirthDate { get; set; }
        
        public ICollection<Visitor> Visitors { get; set; } 
        public Staff? Staff { get; set; } 
    }
}

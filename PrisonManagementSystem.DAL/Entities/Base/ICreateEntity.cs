using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Entities.Base
{
    public interface ICreateEntity : IEntity
    {
        
        public DateTime CreateDate { get; set; }
    }
}

using Microsoft.EntityFrameworkCore.Update;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Entities.Base
{
    public interface IDeleteEntity : IUpdateEntity
    {
      
        public DateTime? DeleteDate { get; set; }
        public bool IsDeleted { get; set; }

    }
}

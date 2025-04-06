using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrisonManagementSystem.DAL.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Configs.Base
{
    public class IBaseEntityConfig<T> : IDeleteConfig<T> where T : BaseEntity
    {
        public override void Configure(EntityTypeBuilder<T> builder)
        {
            base.Configure(builder);
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id);
        }
    }
}

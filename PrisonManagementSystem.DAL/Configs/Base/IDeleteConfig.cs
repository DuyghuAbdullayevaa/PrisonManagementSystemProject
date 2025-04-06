using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrisonManagementSystem.DAL.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Configs.Base
{
    public class IDeleteConfig<T> : IUpdateConfig<T> where T : class, IDeleteEntity
    {
        public override void Configure(EntityTypeBuilder<T> builder)
        {
            base.Configure(builder);
            builder.HasQueryFilter(x => x.IsDeleted == false);
            builder.Property(e => e.IsDeleted).IsRequired();
            builder.Property(e => e.DeleteDate).IsRequired(false);
        }
    }
}

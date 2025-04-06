using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrisonManagementSystem.DAL.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.DAL.Configs.Base
{
    public class ICreateConfig<T> : IEntityConfig<T> where T : class, ICreateEntity
    {
        public override void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(e => e.CreateDate).IsRequired();
        }
    }
}

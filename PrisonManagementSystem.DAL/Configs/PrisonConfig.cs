using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrisonManagementSystem.DAL.Configs.Base;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;

namespace PrisonManagementSystem.DAL.Configs
{
    public class PrisonConfig : IBaseEntityConfig<Prison>
    {
        public override void Configure(EntityTypeBuilder<Prison> builder)
        {
            base.Configure(builder);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Location)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(p => p.Capacity)
                .IsRequired();

            builder.Property(p => p.CurrentInmates)
                .IsRequired();

            builder.Property(p => p.Status)
                .IsRequired();
        }
    }
}

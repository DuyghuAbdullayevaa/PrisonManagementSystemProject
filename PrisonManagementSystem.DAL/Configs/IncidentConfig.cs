using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrisonManagementSystem.DAL.Configs.Base;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;

namespace PrisonManagementSystem.DAL.Configs
{
    public class IncidentConfig : IBaseEntityConfig<Incident>
    {
        public void Configure(EntityTypeBuilder<Incident> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(i => i.IncidentDate)
                .IsRequired();

            builder.HasOne(i => i.Prison)
                .WithMany(p => p.Incidents)
                .HasForeignKey(i => i.PrisonId)
                .OnDelete(DeleteBehavior.NoAction);

            base.Configure(builder);
        }
    }
}

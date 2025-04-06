using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrisonManagementSystem.DAL.Configs.Base;
using PrisonManagementSystem.DAL.Entities.Prison;

namespace PrisonManagementSystem.DAL.Configurations
{
    public class ReportConfig : IBaseEntityConfig<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            base.Configure(builder);

            builder.HasKey(r => r.Id);

            builder.Property(r => r.ReportType)
                   .IsRequired()
                   .HasConversion<int>();

            builder.HasOne(r => r.RelatedIncident)
                   .WithMany(i => i.Reports)
                   .HasForeignKey(r => r.RelatedIncidentId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

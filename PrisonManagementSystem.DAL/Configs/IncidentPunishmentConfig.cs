using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrisonManagementSystem.DAL.Configs.Base;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;

namespace PrisonManagementSystem.DAL.Configs
{
    internal class IncidentPunishmentConfig : IBaseEntityConfig<IncidentPunishment>
    {
        public override void Configure(EntityTypeBuilder<IncidentPunishment> builder)
        {
            base.Configure(builder);

            builder.HasOne(ip => ip.Incident)
                .WithMany(i => i.IncidentPunishments)
                .HasForeignKey(ip => ip.IncidentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ip => ip.Punishment)
                .WithMany(i => i.IncidentPunishments)
                .HasForeignKey(ip => ip.PunishmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

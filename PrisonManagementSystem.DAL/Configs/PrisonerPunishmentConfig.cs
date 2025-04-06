using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Entities.Prison;
using PrisonManagementSystem.DAL.Configs.Base;

namespace PrisonManagementSystem.DAL.Configs
{
    internal class PrisonerPunishmentConfig : IBaseEntityConfig<PrisonerPunishment>
    {
        public override void Configure(EntityTypeBuilder<PrisonerPunishment> builder)
        {
            base.Configure(builder);

            builder.HasKey(pp => new { pp.PrisonerId, pp.PunishmentId });

            builder.HasOne(pp => pp.Prisoner)
                .WithMany(p => p.PrisonerPunishments)
                .HasForeignKey(pp => pp.PrisonerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pp => pp.Punishment)
                .WithMany(p => p.PrisonerPunishments)
                .HasForeignKey(pp => pp.PunishmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrisonManagementSystem.DAL.Configs.Base;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;

namespace PrisonManagementSystem.DAL.Configs
{
    public class PrisonerCrimeConfig : IBaseEntityConfig<PrisonerCrime>
    {
        public void Configure(EntityTypeBuilder<PrisonerCrime> builder)
        {
            builder.HasOne(pc => pc.Prisoner)
                .WithMany(p => p.PrisonerCrimes)
                .HasForeignKey(pc => pc.PrisonerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pc => pc.Crime)
                .WithMany(c => c.PrisonerCrimes)
                .HasForeignKey(pc => pc.CrimeId)
                .OnDelete(DeleteBehavior.Cascade);

            base.Configure(builder);
        }
    }
}

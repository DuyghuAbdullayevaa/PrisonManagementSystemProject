using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrisonManagementSystem.DAL.Configs.Base;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;

namespace PrisonManagementSystem.DAL.Configs
{
    internal class CrimeConfig : IBaseEntityConfig<Crime>
    {
        public override void Configure(EntityTypeBuilder<Crime> builder)
        {
            base.Configure(builder);

            builder.Property(c => c.Details)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(c => c.SeverityLevel)
                .IsRequired();

            builder.Property(c => c.Type)
                .IsRequired();

            builder.Property(c => c.MinimumSentence)
                .IsRequired();

            builder.Property(c => c.MaximumSentence)
                .IsRequired();
        }
    }
}

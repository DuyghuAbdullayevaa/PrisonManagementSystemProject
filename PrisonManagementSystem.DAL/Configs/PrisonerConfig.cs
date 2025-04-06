using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrisonManagementSystem.DAL.Configs.Base;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;

namespace PrisonManagementSystem.DAL.Configs
{
    internal class PrisonerConfig : IBaseEntityConfig<Prisoner>
    {
        public override void Configure(EntityTypeBuilder<Prisoner> builder)
        {
            base.Configure(builder);

            builder.Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.DateOfBirth)
                .IsRequired();

            builder.Property(p => p.Gender)
                .IsRequired();

            builder.Property(p => p.HasPreviousConvictions)
                .IsRequired();

            builder.Property(p => p.AdmissionDate)
                .IsRequired();

            builder.Property(p => p.ReleaseDate)
                .IsRequired(false);

            builder.Property(p => p.Status)
                .IsRequired();

            builder.HasOne(p => p.Cell)
                .WithMany(c => c.Prisoners)
                .HasForeignKey(p => p.CellId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

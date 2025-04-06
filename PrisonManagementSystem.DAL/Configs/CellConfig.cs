using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrisonManagementSystem.DAL.Configs.Base;
using PrisonManagementSystem.DAL.Entities.Base;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;

namespace PrisonManagementSystem.DAL.Configs
{
    internal class CellConfig : IBaseEntityConfig<Cell>
    {
        public override void Configure(EntityTypeBuilder<Cell> builder)
        {
            base.Configure(builder);

            builder.Property(c => c.CellNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Capacity)
                .IsRequired();

            builder.Property(c => c.CurrentOccupancy)
                .IsRequired();

            builder.Property(c => c.Status)
                .IsRequired();

            builder.HasOne(c => c.Prison)
                .WithMany(p => p.Cells)
                .HasForeignKey(c => c.PrisonId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

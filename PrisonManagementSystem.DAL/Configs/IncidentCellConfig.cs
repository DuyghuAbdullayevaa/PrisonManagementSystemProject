using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.DAL.Configs.Base;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;

public class IncidentCellConfig : IBaseEntityConfig<IncidentCell>
{
    public override void Configure(EntityTypeBuilder<IncidentCell> builder)
    {
        base.Configure(builder); // Calling base class configurations

        // CellId configuration
        builder.Property(pi => pi.CellId)
            .IsRequired(); // Required

        // IncidentId configuration
        builder.Property(pi => pi.IncidentId)
            .IsRequired(); // Required

        // Cell relationship configuration
        builder.HasOne(pi => pi.Cell)
            .WithMany(p => p.IncidentCells)
            .HasForeignKey(pi => pi.CellId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent Incident deletion if associated Cell exists

        // Incident relationship configuration
        builder.HasOne(pi => pi.Incident)
            .WithMany(i => i.IncidentCells)
            .HasForeignKey(pi => pi.IncidentId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent Cell deletion if associated Incident exists

        base.Configure(builder);
    }
}

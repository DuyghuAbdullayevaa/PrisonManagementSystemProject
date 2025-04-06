using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.DAL.Configs.Base;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;

public class PrisonerIncidentConfig : IBaseEntityConfig<PrisonerIncident>
{
    public override void Configure(EntityTypeBuilder<PrisonerIncident> builder)
    {
        base.Configure(builder);

        builder.Property(pi => pi.PrisonerId)
            .IsRequired();

        builder.Property(pi => pi.IncidentId)
            .IsRequired();

        builder.HasOne(pi => pi.Prisoner)
            .WithMany(p => p.PrisonersIncidents)
            .HasForeignKey(pi => pi.PrisonerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pi => pi.Incident)
            .WithMany(i => i.PrisonerIncidents)
            .HasForeignKey(pi => pi.IncidentId)
            .OnDelete(DeleteBehavior.Cascade);

        base.Configure(builder);
    }
}

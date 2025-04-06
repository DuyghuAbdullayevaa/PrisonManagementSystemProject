using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using PrisonManagementSystem.DAL.Configs.Base;

public class VisitConfig : IBaseEntityConfig<Visit>
{
    public override void Configure(EntityTypeBuilder<Visit> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.VisitDate)
            .IsRequired();

        builder.Property(v => v.VisitType)
            .IsRequired();

        builder.Property(v => v.DurationInMinutes)
            .IsRequired();

        builder.HasOne(v => v.Prisoner)
            .WithMany(p => p.Visits)
            .HasForeignKey(v => v.PrisonerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(v => v.Visitor)
            .WithMany(vis => vis.VisitHistory)
            .HasForeignKey(v => v.VisitorId)
            .OnDelete(DeleteBehavior.NoAction);

        base.Configure(builder);
    }
}

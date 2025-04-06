using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrisonManagementSystem.DAL.Configs.Base;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;
using System;

namespace PrisonManagementSystem.DAL.Configs
{
    internal class PrisonStaffConfig : IBaseEntityConfig<PrisonStaff>
    {
        public override void Configure(EntityTypeBuilder<PrisonStaff> builder)
        {
            base.Configure(builder);

            builder.HasOne(ps => ps.Prison)
                   .WithMany(p => p.PrisonStaffs)
                   .HasForeignKey(ps => ps.PrisonId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ps => ps.Staff)
                   .WithMany(s => s.PrisonStaffs)
                   .HasForeignKey(ps => ps.StaffId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(ps => ps.PrisonId)
                   .IsRequired();

            builder.Property(ps => ps.StaffId)
                   .IsRequired();
        }
    }
}

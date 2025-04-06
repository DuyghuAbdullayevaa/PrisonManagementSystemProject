using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrisonManagementSystem.DAL.Configs.Base;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;

namespace PrisonManagementSystem.DAL.Configs
{
    public class StaffConfig : IBaseEntityConfig<Staff>
    {
        public void Configure(EntityTypeBuilder<Staff> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.DateOfStarting)
                .IsRequired();

            builder.Property(s => s.PhoneNumber)
                .IsRequired()
                .HasMaxLength(15);

            builder.Property(s => s.Email)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}

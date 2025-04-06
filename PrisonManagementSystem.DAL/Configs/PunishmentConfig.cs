using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrisonManagementSystem.DAL.Configs.Base;
using PrisonManagementSystem.DAL.Entities.PrisonDBContext;

namespace PrisonManagementSystem.DAL.Configs
{
    public class PunishmentConfig : IBaseEntityConfig<Punishment>
    {
        public void Configure(EntityTypeBuilder<Punishment> builder)
        {
            builder.HasKey(p => p.Id);
            base.Configure(builder);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.DAL.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PrisonManagementSystem.DAL.Repositories.Abstractions.Base;
using PrisonManagementSystem.DAL.Repositories.Implementations.Base;

namespace PrisonManagementSystem.DAL.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddDataAccessLayer(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            // Adds the database connection
            serviceCollection.AddDbContext<PrisonDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Adds Unit of Work pattern
            serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();

            // Configures Identity
            serviceCollection.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<PrisonDbContext>()
                .AddDefaultTokenProviders();
        }
    }
}

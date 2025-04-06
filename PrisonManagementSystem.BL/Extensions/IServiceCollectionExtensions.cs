using Microsoft.Extensions.DependencyInjection;
using PrisonManagementSystem.BL.Services.Abstractions;
using PrisonManagementSystem.BL.Services.Abstractions.Identity;
using PrisonManagementSystem.BL.Services.Implementations;
using PrisonManagementSystem.BL.Services.Implementations.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonManagementSystem.BL.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddBusinessLayer(this IServiceCollection services) {
           
            services.AddScoped<IPrisonService, PrisonService>();
            services.AddScoped<IPrisonerService, PrisonerService>();
            services.AddScoped<ICellService, CellService>();
            services.AddScoped<IIncidentService, IncidentService>();
            services.AddScoped<IVisitorService, VisitorService>();
            services.AddScoped<IVisitService, VisitService>();
            services.AddScoped<IPunishmentService, PunishmentService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<ICrimeService, CrimeService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService,TokenService>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


        }
    }
}

using Microsoft.EntityFrameworkCore;
using PrisonManagementSystem.API.Extension;
using PrisonManagementSystem.DAL.Data;
using PrisonManagementSystem.BL.Extensions;
using PrisonManagementSystem.DAL.Extensions;
using FluentValidation.AspNetCore;
using Serilog;
using Microsoft.AspNetCore.Identity;
using PrisonManagementSystem.DAL.Entities.Identity;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

namespace PrisonManagementSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // FluentValidation Configuration
            builder.Services.AddFluentValidation(config =>
            {
                config.RegisterValidatorsFromAssemblyContaining<CreatePrisonDtoValidator>();
            });

            // Controller Configuration
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                });

            // Custom Validation Response
            builder.Services.UseCustomValidationResponse();

            // Business Layer and Data Access Layer Configuration
            builder.Services.AddBusinessLayer();
            builder.Services.AddDataAccessLayer(builder.Configuration);

            // Serilog Logging Configuration
            builder.AddSerilogLogging();

            // Swagger Configuration
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerDocumentation();

            // JWT Authentication Configuration
            builder.Services.AddJwtAuthentication(builder.Configuration);

            var app = builder.Build();

            // Development Environment: Swagger
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Serilog Request Logging
            app.UseSerilogRequestLogging();

            // HTTPS Redirection and Exception Handling
            app.UseHttpsRedirection();
            app.ConfigureExceptionHandler();

            // Authentication and Authorization Middleware
            app.UseAuthentication();
            app.UseAuthorization();

            // Map Controllers
            app.MapControllers();

            app.Run();
        }
    }
}

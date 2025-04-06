using Serilog;

namespace PrisonManagementSystem.API.Extension
{
    public static class SerilogExtension
    {
        public static void AddSerilogLogging(this WebApplicationBuilder builder)
        {
            var configuration = builder.Configuration;

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)  
                .Enrich.FromLogContext()  // Adds additional context data to logs
                .CreateLogger(); // Create the logger

            builder.Host.UseSerilog(); // Use Serilog as the logging provider
        }
    }
}

using Microsoft.AspNetCore.Diagnostics;
using PrisonManagementSystem.BL.DTOs.ResponseModel;
using Serilog;
using System.Net;
using System.Text.Json;

namespace PrisonManagementSystem.API.Extension
{
    public static class ExceptionHandlingExtension
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (contextFeature != null)
                    {
                        var exception = contextFeature.Error;
                        var statusCode = GetStatusCode(exception);
                        context.Response.StatusCode = (int)statusCode;

                        var errorResponse = GenericResponseModel<string>.FailureResponse(
                            GetErrorMessage(exception), (int)statusCode);

                        Log.Error($"Error: {exception.Message} | Path: {context.Request.Path}");

                        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
                    }
                });
            });
        }

        private static HttpStatusCode GetStatusCode(Exception exception)
        {
            return exception switch
            {
                ArgumentException => HttpStatusCode.BadRequest,
                KeyNotFoundException => HttpStatusCode.NotFound,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError
            };
        }

        private static string GetErrorMessage(Exception exception)
        {
            return exception switch
            {
                ArgumentException => "Invalid request parameters.",
                KeyNotFoundException => "Requested resource was not found.",
                UnauthorizedAccessException => "You are not authorized to access this resource.",
                _ => "An unexpected error occurred. Please try again later."
            };
        }
    }
}

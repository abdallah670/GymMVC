using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GymPL.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Check if request is an API request or expects JSON
            var isApiRequest = context.Request.Path.StartsWithSegments("/api") || 
                              context.Request.Headers["Accept"].ToString().Contains("application/json");

            if (isApiRequest)
            {
                var response = new ErrorResponse
                {
                    StatusCode = context.Response.StatusCode,
                    Message = _env.IsDevelopment() ? exception.Message : "An internal server error occurred.",
                    Details = _env.IsDevelopment() ? exception.StackTrace : null
                };

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }
            else
            {
                // For regular MVC requests, redirect to error page
                // But first check if we are not already on the error page to avoid infinite loops
                if (!context.Request.Path.StartsWithSegments("/Home/Error"))
                {
                    context.Response.Redirect("/Home/Error");
                }
                else
                {
                    // Fallback if already on error page
                    await context.Response.WriteAsync("A critical error occurred. Please try again later.");
                }
            }
        }
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string? Details { get; set; }
    }

    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}

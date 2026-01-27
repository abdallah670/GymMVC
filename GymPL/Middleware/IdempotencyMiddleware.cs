using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GymPL.Middleware
{
    /// <summary>
    /// Middleware to check for Idempotency-Key header on payment-related endpoints.
    /// For now, logs a warning if the header is missing. 
    /// Full implementation would cache responses by key.
    /// </summary>
    public class IdempotencyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<IdempotencyMiddleware> _logger;

        public IdempotencyMiddleware(RequestDelegate next, ILogger<IdempotencyMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if this is a payment-related endpoint
            var path = context.Request.Path.Value?.ToLower() ?? "";
            if ((path.Contains("/api/stripewebhook") || path.Contains("/payment")) && context.Request.Method == "POST")
            {
                if (!context.Request.Headers.ContainsKey("X-Idempotency-Key"))
                {
                    _logger.LogWarning("Payment request to {Path} is missing X-Idempotency-Key header.", path);
                    // In a full implementation, you could reject the request here
                    // return context.Response.WriteAsync("Missing Idempotency-Key");
                }
                else
                {
                    var idempotencyKey = context.Request.Headers["X-Idempotency-Key"].ToString();
                    _logger.LogInformation("Payment request with Idempotency-Key: {Key}", idempotencyKey);
                    // TODO: Check cache for existing response with this key
                    // If found, return cached response
                    // If not, proceed and cache the response
                }
            }

            await _next(context);
        }
    }

    public static class IdempotencyMiddlewareExtensions
    {
        public static IApplicationBuilder UseIdempotencyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IdempotencyMiddleware>();
        }
    }
}

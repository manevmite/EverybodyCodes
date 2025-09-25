using System.Net;
using System.Text.Json;

namespace EveryoneCodes.Api.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                error = new
                {
                    message = GetErrorMessage(exception),
                    details = GetErrorDetails(exception),
                    timestamp = DateTime.UtcNow,
                    traceId = context.TraceIdentifier
                }
            };

            var statusCode = GetStatusCode(exception);
            context.Response.StatusCode = (int)statusCode;

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            await context.Response.WriteAsync(jsonResponse);
        }

        private static string GetErrorMessage(Exception exception)
        {
            return exception switch
            {
                ArgumentNullException => "A required parameter is missing.",
                ArgumentException => "An invalid parameter was provided.",
                FileNotFoundException => "The requested resource was not found.",
                UnauthorizedAccessException => "Access to the requested resource is denied.",
                TimeoutException => "The operation timed out.",
                _ => "An unexpected error occurred while processing your request."
            };
        }

        private static string? GetErrorDetails(Exception exception)
        {
            // In production, you might want to return more details based on environment
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                ? exception.ToString()
                : null;
        }

        private static HttpStatusCode GetStatusCode(Exception exception)
        {
            return exception switch
            {
                ArgumentNullException or ArgumentException => HttpStatusCode.BadRequest,
                FileNotFoundException => HttpStatusCode.NotFound,
                UnauthorizedAccessException => HttpStatusCode.Forbidden,
                TimeoutException => HttpStatusCode.RequestTimeout,
                _ => HttpStatusCode.InternalServerError
            };
        }
    }
}

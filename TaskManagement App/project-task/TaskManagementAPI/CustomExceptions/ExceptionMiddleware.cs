using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using TaskManagementAPI.CustomExceptions;

namespace TaskManagementAPI.CustomExceptions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint()?.DisplayName ?? context.Request.Path;
            var userId = context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
            var method = context.Request.Method;
            var requestTime = DateTime.UtcNow;

            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context);
                stopwatch.Stop();

                _logger.LogInformation("Request {Method} {Endpoint} by UserId: {UserId} at {Timestamp} completed with StatusCode {StatusCode} in {Duration} ms",
                    method, endpoint, userId, requestTime, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
            }
            catch (ApiException ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "API Exception on {Method} {Endpoint} by UserId: {UserId} at {Timestamp} after {Duration} ms",
                    method, endpoint, userId, requestTime, stopwatch.ElapsedMilliseconds);

                await WriteJsonResponseAsync(context, ex.StatusCode, ex.Message);
            }
            catch (BadRequestException ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "BadRequestException on {Method} {Endpoint} by UserId: {UserId} at {Timestamp} after {Duration} ms",
                    method, endpoint, userId, requestTime, stopwatch.ElapsedMilliseconds);

                await WriteJsonResponseAsync(context, (int)HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Unhandled error on {Method} {Endpoint} by UserId: {UserId} at {Timestamp} after {Duration} ms",
                    method, endpoint, userId, requestTime, stopwatch.ElapsedMilliseconds);

                await WriteJsonResponseAsync(context, (int)HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }
        }

        private async Task WriteJsonResponseAsync(HttpContext context, int statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var errorObj = new
            {
                success = false,
                message = message,
                statusCode = statusCode
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorObj, options));
        }
    }
}

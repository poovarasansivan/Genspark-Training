using System.Text.Json;
using FitnessTracking.Misc;
using FitnessTracking.Models.DTOs;

namespace FitnessTracking.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate? _next;
        private readonly ILogger<ExceptionMiddleware>? _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (CustomeExceptionHandler ex)
            {
                await HandleExceptionAsyc(context, ex.StatusCode, ex.Message, ex.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled Exception");
                await HandleExceptionAsyc(context, 500, "Internal Server Error");
            }
        }

        private async Task HandleExceptionAsyc(HttpContext context, int statusCode, string message, Dictionary<string, string[]>? errors = null)
        {
            var response = new ApiErrorResponseDto
            {
                Message = message,
                Errors = errors
            };
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
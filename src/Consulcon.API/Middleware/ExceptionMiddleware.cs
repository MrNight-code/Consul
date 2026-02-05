using Consulcon.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Consulcon.API.Middleware
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
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // 1. Get TraceId
            var traceId = context.TraceIdentifier;

            // 2. Log full context with Serilog
            _logger.LogError(exception, "Error Code: {ErrorCode}, TraceId: {TraceId}, Message: {Message}", 
                GetErrorCode(exception), traceId, exception.Message);

            // 3. Determine Status Code and Error Code
            var statusCode = DetermineStatusCode(exception);
            var errorCode = GetErrorCode(exception);
            var message = GetUserFriendlyMessage(exception);

            if (statusCode == HttpStatusCode.InternalServerError)
            {
                 // Mask internal server errors in production if desired, but requirements asked for descriptive message explanation.
                 // We will stick to the mapped message.
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            // 4. Construct Response matching Result<T> structure + ErrorResponse requirements
            var response = new
            {
                IsSuccess = false,
                IsFailure = true,
                ErrorCode = errorCode,
                Message = message,
                TraceId = traceId,
                Value = (object?)null,
                Timestamp = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            await context.Response.WriteAsync(json);
        }

        private static HttpStatusCode DetermineStatusCode(Exception exception)
        {
            return exception switch
            {
                ArgumentException 
                or ArgumentNullException 
                or FormatException         => HttpStatusCode.BadRequest,
                
                KeyNotFoundException 
                or FileNotFoundException   => HttpStatusCode.NotFound,
                
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                
                NotImplementedException    => HttpStatusCode.NotImplemented,
                
                // Add your custom exceptions here if any
                // BusinessRuleException => HttpStatusCode.BadRequest,

                _ => HttpStatusCode.InternalServerError
            };
        }

        private static string GetErrorCode(Exception exception)
        {
            return exception switch
            {
                ArgumentNullException       => "ERR-ARG-001",
                ArgumentException           => "ERR-ARG-002",
                FormatException             => "ERR-FMT-001",
                KeyNotFoundException        => "ERR-DAT-001",
                FileNotFoundException       => "ERR-FIL-001",
                UnauthorizedAccessException => "ERR-AUTH-001",
                NotImplementedException     => "ERR-SYS-001",
                // Custom or Database exceptions
                // SqlException => "ERR-DB-001", 
                _ => "ERR-GEN-500"
            };
        }

        private static string GetUserFriendlyMessage(Exception exception)
        {
             return exception switch
            {
                ArgumentNullException       => "Valor requerido no proporcionado.",
                ArgumentException           => exception.Message, // Safe to return usually
                FormatException             => "El formato de los datos es incorrecto.",
                KeyNotFoundException        => "Recurso no encontrado.",
                FileNotFoundException       => "Archivo no encontrado.",
                UnauthorizedAccessException => "No tiene permisos para realizar esta acción.",
                NotImplementedException     => "Funcionalidad no disponible aún.",
                _ => "Ha ocurrido un error inesperado en el sistema."
            };
        }
    }
}

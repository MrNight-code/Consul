using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Consulcon.Application.Interfaces.Contabilidad;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Consulcon.API.Middleware;

/// <summary>
/// Middleware que valida si un período fiscal está cerrado antes de permitir
/// operaciones de escritura (POST, PUT, DELETE) en endpoints de egresos.
/// </summary>
public class PeriodLockMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PeriodLockMiddleware> _logger;

    // Rutas que requieren validación de período fiscal
    private static readonly string[] ProtectedRoutes =
    {
        "/api/expenses",
        "/api/tesoreria/egresos"
    };

    // Métodos HTTP que modifican datos
    private static readonly string[] ModifyingMethods = { "POST", "PUT", "DELETE" };

    public PeriodLockMiddleware(RequestDelegate next, ILogger<PeriodLockMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IFiscalPeriodService fiscalPeriodService)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
        var method = context.Request.Method.ToUpperInvariant();

        // Solo validar rutas protegidas con métodos que modifican datos
        if (!IsProtectedRoute(path) || !ModifyingMethods.Contains(method))
        {
            await _next(context);
            return;
        }

        // Intentar extraer condominioId y fecha del request
        var (success, condominioId, expenseDate, errorMessage) = await TryExtractPeriodInfoAsync(context);

        if (!success)
        {
            // Si no podemos extraer la información, dejamos pasar (la validación la hará el controller)
            _logger.LogDebug("PeriodLockMiddleware: No se pudo extraer información del período, continuando: {Error}", errorMessage);
            await _next(context);
            return;
        }

        // Verificar si el período está cerrado
        if (fiscalPeriodService.IsPeriodClosed(condominioId, expenseDate))
        {
            _logger.LogWarning(
                "Intento de modificar egreso en período cerrado. CondominioId: {CondominioId}, Fecha: {Date}, Período: {Month}/{Year}",
                condominioId, expenseDate, expenseDate.Month, expenseDate.Year);

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var response = new
            {
                IsSuccess = false,
                IsFailure = true,
                ErrorCode = "PERIOD_CLOSED",
                Message = $"El período {expenseDate.Month:D2}/{expenseDate.Year} está cerrado. No se pueden agregar, modificar o eliminar egresos en períodos cerrados.",
                Period = new { Year = expenseDate.Year, Month = expenseDate.Month }
            };

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
            return;
        }

        await _next(context);
    }

    private static bool IsProtectedRoute(string path)
    {
        return ProtectedRoutes.Any(route => path.StartsWith(route, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<(bool Success, int CondominioId, DateTime ExpenseDate, string? Error)> TryExtractPeriodInfoAsync(HttpContext context)
    {
        try
        {
            // Habilitar la lectura múltiple del body
            context.Request.EnableBuffering();

            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();

            // Resetear el stream para que el controller pueda leerlo
            context.Request.Body.Position = 0;

            if (string.IsNullOrWhiteSpace(body))
                return (false, 0, default, "Body vacío");

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            // Intentar obtener CondominioId
            int condominioId = 0;
            if (root.TryGetProperty("condominioId", out var condProp) ||
                root.TryGetProperty("CondominioId", out condProp) ||
                root.TryGetProperty("IdCondominio", out condProp))
            {
                condominioId = condProp.GetInt32();
            }

            if (condominioId <= 0)
                return (false, 0, default, "CondominioId no encontrado o inválido");

            // Intentar obtener la fecha del egreso
            DateTime expenseDate = DateTime.UtcNow; // Default: fecha actual

            if (root.TryGetProperty("expenseDate", out var dateProp) ||
                root.TryGetProperty("ExpenseDate", out dateProp) ||
                root.TryGetProperty("fechaEgreso", out dateProp) ||
                root.TryGetProperty("FechaEgreso", out dateProp))
            {
                if (dateProp.ValueKind == JsonValueKind.String)
                {
                    if (DateTime.TryParse(dateProp.GetString(), out var parsedDate))
                    {
                        expenseDate = parsedDate;
                    }
                }
            }

            return (true, condominioId, expenseDate, null);
        }
        catch (Exception ex)
        {
            return (false, 0, default, ex.Message);
        }
    }
}

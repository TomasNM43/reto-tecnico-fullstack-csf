using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Errors;

// Atrapa cualquier excepción no controlada y responde con un ProblemDetails (JSON estándar),
// para que el cliente nunca vea el stack trace.
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            _logger.LogError(ex, "Error no controlado en {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteProblemAsync(context, ex);
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, Exception ex)
    {
        var (status, title, detail) = ex switch
        {
            // Mientras la lógica de negocio esté pendiente, se avisa claramente.
            NotImplementedException => (HttpStatusCode.NotImplemented, "Lógica pendiente", "Esta operación aún no está implementada."),
            ArgumentException or InvalidOperationException => (HttpStatusCode.BadRequest, "Solicitud inválida", ex.Message),
            KeyNotFoundException => (HttpStatusCode.NotFound, "No encontrado", ex.Message),
            _ => (HttpStatusCode.InternalServerError, "Error interno", "Ocurrió un error inesperado.")
        };

        if (context.Response.HasStarted) return;

        context.Response.Clear();
        context.Response.StatusCode = (int)status;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = (int)status,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        });
    }
}

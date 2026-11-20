using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Security;

// Agrega las cabeceras de seguridad HTTP a todas las respuestas y oculta la cabecera Server.
public class SecurityHeadersMiddleware
{
    // La API solo devuelve JSON, así que no necesita cargar nada.
    private const string ApiCsp = "default-src 'none'; frame-ancestors 'none'";

    // Swagger UI usa scripts y estilos propios, por eso tiene una política más abierta.
    private const string SwaggerCsp =
        "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:; frame-ancestors 'none'";

    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next) => _next = next;

    public Task InvokeAsync(HttpContext context)
    {
        // OnStarting asegura que las cabeceras se pongan aunque la respuesta venga de otro middleware.
        context.Response.OnStarting(() =>
        {
            var headers = context.Response.Headers;
            var esSwagger = context.Request.Path.StartsWithSegments("/swagger");

            headers["X-Content-Type-Options"] = "nosniff";
            headers["X-Frame-Options"] = "DENY";
            headers["Referrer-Policy"] = "no-referrer";
            headers["Content-Security-Policy"] = esSwagger ? SwaggerCsp : ApiCsp;
            headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
            headers.Remove("Server");
            return Task.CompletedTask;
        });

        return _next(context);
    }
}

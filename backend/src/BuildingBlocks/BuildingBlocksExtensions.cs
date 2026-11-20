using BuildingBlocks.Errors;
using BuildingBlocks.Security;
using BuildingBlocks.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BuildingBlocks;

// Punto único para activar toda la seguridad transversal en cada servicio.
public static class BuildingBlocksExtensions
{
    public static WebApplicationBuilder AddBuildingBlocks(this WebApplicationBuilder builder, string titulo)
    {
        // Quita la cabecera Server de Kestrel.
        builder.WebHost.ConfigureKestrel(kestrel => kestrel.AddServerHeader = false);

        builder.Services
            .AddJwtAuthentication(builder.Configuration)
            .AddStrictCors(builder.Configuration)
            .AddRateLimiting(builder.Configuration)
            .AddSwaggerWithJwt(titulo);

        return builder;
    }

    // El orden importa: errores -> cabeceras -> CORS -> rate limit -> autenticación -> autorización.
    public static WebApplication UseBuildingBlocks(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionMiddleware>();
        app.UseMiddleware<SecurityHeadersMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStrictCors();
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}

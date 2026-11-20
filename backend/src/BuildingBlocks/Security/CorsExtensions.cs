using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Security;

public static class CorsExtensions
{
    // CORS estricto: solo los orígenes configurados, nunca "*".
    public static IServiceCollection AddStrictCors(this IServiceCollection services, IConfiguration config)
    {
        var options = config.GetSection(CorsOptions.SectionName).Get<CorsOptions>() ?? new CorsOptions();

        services.AddCors(cors => cors.AddPolicy(CorsOptions.PolicyName, policy =>
        {
            if (options.AllowedOrigins.Length == 0) return; // sin orígenes configurados, nadie pasa

            policy.WithOrigins(options.AllowedOrigins)
                  .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                  .WithHeaders("Authorization", "Content-Type");
        }));

        return services;
    }

    public static IApplicationBuilder UseStrictCors(this IApplicationBuilder app)
        => app.UseCors(CorsOptions.PolicyName);
}

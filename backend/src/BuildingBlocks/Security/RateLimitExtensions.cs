using System.Net;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Security;

public static class RateLimitExtensions
{
    // Límite general por IP, más una política "login" mucho más estricta.
    public static IServiceCollection AddRateLimiting(this IServiceCollection services, IConfiguration config)
    {
        var options = config.GetSection(RateLimitOptions.SectionName).Get<RateLimitOptions>() ?? new RateLimitOptions();

        services.AddRateLimiter(limiter =>
        {
            limiter.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            limiter.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(GetClientKey(context), _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = options.PermitLimit,
                    Window = TimeSpan.FromSeconds(options.WindowSeconds),
                    QueueLimit = 0
                }));

            limiter.AddPolicy(RateLimitOptions.LoginPolicy, context =>
                RateLimitPartition.GetFixedWindowLimiter(GetClientKey(context), _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = options.LoginPermitLimit,
                    Window = TimeSpan.FromSeconds(options.LoginWindowSeconds),
                    QueueLimit = 0
                }));

            limiter.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.ContentType = "application/problem+json";
                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    status = 429,
                    title = "Demasiadas peticiones",
                    detail = "Superaste el límite permitido. Intenta de nuevo en unos segundos."
                }, token);
            };
        });

        return services;
    }

    private static string GetClientKey(HttpContext context)
        => context.Connection.RemoteIpAddress?.ToString() ?? IPAddress.None.ToString();
}

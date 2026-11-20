using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Http;

public static class ServiceClientExtensions
{
    // Registra un cliente HTTP tipado hacia otro microservicio, reenviando el JWT del usuario.
    public static IHttpClientBuilder AddServiceClient<TInterfaz, TImplementacion>(
        this IServiceCollection services, string baseUrl)
        where TInterfaz : class
        where TImplementacion : class, TInterfaz
    {
        services.AddHttpContextAccessor();
        services.AddTransient<BearerTokenForwardingHandler>();

        return services
            .AddHttpClient<TInterfaz, TImplementacion>(cliente =>
            {
                cliente.BaseAddress = new Uri(baseUrl);
                cliente.Timeout = TimeSpan.FromSeconds(15);
            })
            .AddHttpMessageHandler<BearerTokenForwardingHandler>();
    }
}

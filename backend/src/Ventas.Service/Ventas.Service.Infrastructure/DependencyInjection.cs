using BuildingBlocks.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ventas.Service.Application.Configuration;
using Ventas.Service.Application.Interfaces;
using Ventas.Service.Application.Services;
using Ventas.Service.Infrastructure.Clients;
using Ventas.Service.Infrastructure.Persistence;

namespace Ventas.Service.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddVentasServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<VentasDbContext>(o => o.UseNpgsql(config.GetConnectionString("Default")));

        services.AddSingleton(config.GetSection(ConfiguracionNegocio.SectionName).Get<ConfiguracionNegocio>() ?? new ConfiguracionNegocio());

        services.AddScoped<IVentaRepository, VentaRepository>();
        services.AddScoped<IVentaService, VentaService>();

        var urlProductos = config["Services:Productos"]
            ?? throw new InvalidOperationException("Falta la configuración Services:Productos.");
        var urlMovimientos = config["Services:Movimientos"]
            ?? throw new InvalidOperationException("Falta la configuración Services:Movimientos.");

        services.AddServiceClient<IProductosFacade, ProductosHttpFacade>(urlProductos);

        // La Facade real se registra por su tipo concreto y se entrega envuelta en el Decorator de logging.
        services.AddServiceClient<MovimientosHttpFacade, MovimientosHttpFacade>(urlMovimientos);
        services.AddScoped<IMovimientosFacade>(sp => new LoggingMovimientosFacade(
            sp.GetRequiredService<MovimientosHttpFacade>(),
            sp.GetRequiredService<ILogger<LoggingMovimientosFacade>>()));

        return services;
    }
}

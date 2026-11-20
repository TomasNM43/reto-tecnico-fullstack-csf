using Compras.Service.Application.Interfaces;
using Compras.Service.Infrastructure.Clients;
using Microsoft.Extensions.Logging;
using Compras.Service.Application.Configuration;
using Compras.Service.Application.Services;
using Compras.Service.Infrastructure.Persistence;
using BuildingBlocks.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Compras.Service.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddComprasServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<ComprasDbContext>(o => o.UseNpgsql(config.GetConnectionString("Default")));

        services.AddSingleton(config.GetSection(ConfiguracionNegocio.SectionName).Get<ConfiguracionNegocio>() ?? new ConfiguracionNegocio());

        services.AddScoped<ICompraRepository, CompraRepository>();
        services.AddScoped<ICompraService, CompraService>();

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

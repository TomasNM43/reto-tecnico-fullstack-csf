using BuildingBlocks.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Movimientos.Service.Application.Interfaces;
using Movimientos.Service.Application.Services;
using Movimientos.Service.Infrastructure.Clients;
using Movimientos.Service.Infrastructure.Persistence;

namespace Movimientos.Service.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMovimientosServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<MovimientosDbContext>(o => o.UseNpgsql(config.GetConnectionString("Default")));

        services.AddScoped<IMovimientoRepository, MovimientoRepository>();
        services.AddScoped<IMovimientoService, MovimientoService>();
        services.AddScoped<IStockService, StockService>();
        services.AddScoped<IKardexService, KardexService>();

        var urlProductos = config["Services:Productos"]
            ?? throw new InvalidOperationException("Falta la configuración Services:Productos.");
        services.AddServiceClient<IProductosFacade, ProductosHttpFacade>(urlProductos);

        return services;
    }
}

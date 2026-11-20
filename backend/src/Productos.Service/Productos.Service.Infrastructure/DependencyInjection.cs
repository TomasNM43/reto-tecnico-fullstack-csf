using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Productos.Service.Application.Interfaces;
using Productos.Service.Application.Services;
using Productos.Service.Infrastructure.Persistence;

namespace Productos.Service.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddProductosServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<ProductosDbContext>(o => o.UseNpgsql(config.GetConnectionString("Default")));

        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<IProductoService, ProductoService>();

        return services;
    }
}

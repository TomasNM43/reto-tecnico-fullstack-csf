using Microsoft.Extensions.Logging;
using Ventas.Service.Application.Dtos;
using Ventas.Service.Application.Interfaces;

namespace Ventas.Service.Infrastructure.Clients;

// Patrón Decorator: envuelve la Facade real y le agrega logging sin modificarla.
public class LoggingMovimientosFacade : IMovimientosFacade
{
    private readonly IMovimientosFacade _interna;
    private readonly ILogger<LoggingMovimientosFacade> _logger;

    public LoggingMovimientosFacade(IMovimientosFacade interna, ILogger<LoggingMovimientosFacade> logger)
    {
        _interna = interna;
        _logger = logger;
    }

    public async Task<int> ObtenerStockAsync(int idProducto, CancellationToken ct = default)
    {
        _logger.LogInformation("Consultando stock del producto {IdProducto}", idProducto);
        var stock = await _interna.ObtenerStockAsync(idProducto, ct);
        _logger.LogInformation("Stock del producto {IdProducto}: {Stock}", idProducto, stock);
        return stock;
    }

    public async Task RegistrarSalidaAsync(int idVentaCab, IReadOnlyList<MovimientoItemDto> items, CancellationToken ct = default)
    {
        _logger.LogInformation("Registrando movimiento de Salida para la venta {IdVenta} con {Cantidad} producto(s)", idVentaCab, items.Count);

        try
        {
            await _interna.RegistrarSalidaAsync(idVentaCab, items, ct);
            _logger.LogInformation("Movimiento de Salida registrado para la venta {IdVenta}", idVentaCab);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falló el movimiento de Salida de la venta {IdVenta}", idVentaCab);
            throw;
        }
    }
}

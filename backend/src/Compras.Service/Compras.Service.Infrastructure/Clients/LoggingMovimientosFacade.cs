using Compras.Service.Application.Dtos;
using Compras.Service.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Compras.Service.Infrastructure.Clients;

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

    public async Task RegistrarEntradaAsync(int idCompraCab, IReadOnlyList<MovimientoItemDto> items, CancellationToken ct = default)
    {
        _logger.LogInformation("Registrando movimiento de Entrada para la compra {IdCompra} con {Cantidad} producto(s)", idCompraCab, items.Count);

        try
        {
            await _interna.RegistrarEntradaAsync(idCompraCab, items, ct);
            _logger.LogInformation("Movimiento de Entrada registrado para la compra {IdCompra}", idCompraCab);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falló el movimiento de Entrada de la compra {IdCompra}", idCompraCab);
            throw;
        }
    }
}

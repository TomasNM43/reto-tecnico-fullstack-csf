using Compras.Service.Application.Dtos;

namespace Compras.Service.Application.Interfaces;

// Facade hacia Movimientos.Service.
public interface IMovimientosFacade
{
    // Registra un movimiento de Entrada asociado a la compra indicada.
    Task RegistrarEntradaAsync(int idCompraCab, IReadOnlyList<MovimientoItemDto> items, CancellationToken ct = default);
}

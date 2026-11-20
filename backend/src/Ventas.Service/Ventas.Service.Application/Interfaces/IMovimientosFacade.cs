using Ventas.Service.Application.Dtos;

namespace Ventas.Service.Application.Interfaces;

// Facade hacia Movimientos.Service.
public interface IMovimientosFacade
{
    // Stock actual del producto, calculado por Movimientos.Service a partir de sus movimientos.
    Task<int> ObtenerStockAsync(int idProducto, CancellationToken ct = default);

    // Registra un movimiento de Salida asociado a la venta indicada.
    Task RegistrarSalidaAsync(int idVentaCab, IReadOnlyList<MovimientoItemDto> items, CancellationToken ct = default);
}

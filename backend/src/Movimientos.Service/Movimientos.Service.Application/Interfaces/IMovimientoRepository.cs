using Movimientos.Service.Application.Dtos;
using Movimientos.Service.Domain.Entities;

namespace Movimientos.Service.Application.Interfaces;

public interface IMovimientoRepository
{
    // Guarda la cabecera y su detalle en una sola transacción.
    Task AgregarAsync(MovimientoCab movimiento, CancellationToken ct = default);

    // Movimientos de un solo producto, del más antiguo al más reciente.
    Task<IReadOnlyList<MovimientoPlano>> ListarPorProductoAsync(int idProducto, CancellationToken ct = default);

    // Todos los movimientos de todos los productos.
    Task<IReadOnlyList<MovimientoPlano>> ListarTodosAsync(CancellationToken ct = default);
}

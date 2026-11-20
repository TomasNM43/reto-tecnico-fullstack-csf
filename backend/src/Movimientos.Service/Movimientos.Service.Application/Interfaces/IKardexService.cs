using Movimientos.Service.Application.Dtos;

namespace Movimientos.Service.Application.Interfaces;

public interface IKardexService
{
    // Tabla del Kardex: un renglón por producto con su stock actual.
    Task<IReadOnlyList<KardexItemDto>> ListarKardexAsync(CancellationToken ct = default);

    // Movimientos de un producto para el modal de cada fila.
    Task<IReadOnlyList<MovimientoProductoDto>> ListarMovimientosAsync(int idProducto, CancellationToken ct = default);
}

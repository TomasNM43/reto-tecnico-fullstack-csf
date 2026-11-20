using Movimientos.Service.Application.Dtos;
using Movimientos.Service.Application.Interfaces;

namespace Movimientos.Service.Application.Services;

public class KardexService : IKardexService
{
    private readonly IMovimientoRepository _movimientos;
    private readonly IProductosFacade _productos;

    public KardexService(IMovimientoRepository movimientos, IProductosFacade productos)
    {
        _movimientos = movimientos;
        _productos = productos;
    }

    public Task<IReadOnlyList<KardexItemDto>> ListarKardexAsync(CancellationToken ct = default)
    {
        // TODO(USUARIO): armar la tabla del Kardex con un renglón por producto y su stock actual.
        // REGLA: cada fila muestra Id_producto, nombre_producto, stock_actual, costo y precio de venta;
        //        el stock sale de los movimientos (Entradas suman, Salidas restan).
        // PISTA: _productos.ListarAsync() trae los productos (nombre, costo, precio);
        //        _movimientos.ListarTodosAsync() trae todos los movimientos, agrúpalos por IdProducto
        //        para calcular el stock de cada uno y combínalos en KardexItemDto.
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<MovimientoProductoDto>> ListarMovimientosAsync(int idProducto, CancellationToken ct = default)
    {
        var movimientos = await _movimientos.ListarPorProductoAsync(idProducto, ct);

        return movimientos
            .Select(m => new MovimientoProductoDto(m.FecRegistro, (int)m.Tipo, m.Tipo.ToString(), m.Cantidad))
            .ToList();
    }
}

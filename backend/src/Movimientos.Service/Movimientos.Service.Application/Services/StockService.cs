using Movimientos.Service.Application.Interfaces;

namespace Movimientos.Service.Application.Services;

public class StockService : IStockService
{
    private readonly IMovimientoRepository _movimientos;

    public StockService(IMovimientoRepository movimientos) => _movimientos = movimientos;

    public Task<int> ObtenerStockAsync(int idProducto, CancellationToken ct = default)
    {
        // TODO(USUARIO): calcular el stock actual del producto a partir de sus movimientos.
        // REGLA: el stock se calcula desde los movimientos: Entradas suman y Salidas restan.
        // PISTA: _movimientos.ListarPorProductoAsync(idProducto) devuelve los movimientos del producto;
        //        recorre la lista y acumula según Tipo (TipoMovimiento.Entrada / TipoMovimiento.Salida).
        throw new NotImplementedException();
    }
}

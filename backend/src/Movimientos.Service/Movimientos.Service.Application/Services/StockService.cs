using Movimientos.Service.Application.Interfaces;

namespace Movimientos.Service.Application.Services;

public class StockService : IStockService
{
    private readonly IMovimientoRepository _movimientos;

    public StockService(IMovimientoRepository movimientos) => _movimientos = movimientos;

    public async Task<int> ObtenerStockAsync(int idProducto, CancellationToken ct = default)
    {
        var movimientos = await _movimientos.ListarPorProductoAsync(idProducto, ct);
        return CalculoStock.Calcular(movimientos);
    }
}

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

    public async Task<IReadOnlyList<KardexItemDto>> ListarKardexAsync(CancellationToken ct = default)
    {
        var productos = await _productos.ListarAsync(ct);
        var movimientos = await _movimientos.ListarTodosAsync(ct);

        // Un solo recorrido de los movimientos: el stock de cada producto sale de agruparlos por producto.
        var stockPorProducto = movimientos
            .GroupBy(m => m.IdProducto)
            .ToDictionary(g => g.Key, g => CalculoStock.Calcular(g));

        return productos
            .OrderBy(p => p.IdProducto)
            .Select(p => new KardexItemDto(
                p.IdProducto,
                p.NombreProducto,
                stockPorProducto.GetValueOrDefault(p.IdProducto),
                p.Costo,
                p.PrecioVenta))
            .ToList();
    }

    public async Task<IReadOnlyList<MovimientoProductoDto>> ListarMovimientosAsync(int idProducto, CancellationToken ct = default)
    {
        var movimientos = await _movimientos.ListarPorProductoAsync(idProducto, ct);

        return movimientos
            .Select(m => new MovimientoProductoDto(m.FecRegistro, (int)m.Tipo, m.Tipo.ToString(), m.Cantidad))
            .ToList();
    }
}

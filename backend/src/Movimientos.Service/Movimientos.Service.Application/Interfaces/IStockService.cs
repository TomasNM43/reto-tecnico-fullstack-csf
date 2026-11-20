namespace Movimientos.Service.Application.Interfaces;

public interface IStockService
{
    // Stock actual de un producto, calculado a partir de sus movimientos.
    Task<int> ObtenerStockAsync(int idProducto, CancellationToken ct = default);
}

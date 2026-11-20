using Productos.Service.Domain.Entities;

namespace Productos.Service.Application.Interfaces;

public interface IProductoRepository
{
    Task<IReadOnlyList<Producto>> ListarAsync(CancellationToken ct = default);
    Task<Producto?> ObtenerPorIdAsync(int idProducto, CancellationToken ct = default);
    Task AgregarAsync(Producto producto, CancellationToken ct = default);
    Task ActualizarAsync(Producto producto, CancellationToken ct = default);
}

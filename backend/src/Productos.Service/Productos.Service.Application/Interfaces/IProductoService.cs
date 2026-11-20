using Productos.Service.Application.Dtos;

namespace Productos.Service.Application.Interfaces;

public interface IProductoService
{
    Task<IReadOnlyList<ProductoResponse>> ListarAsync(CancellationToken ct = default);

    // Lanza KeyNotFoundException si el producto no existe.
    Task<ProductoResponse> ObtenerAsync(int idProducto, CancellationToken ct = default);

    Task<ProductoResponse> RegistrarAsync(ProductoRequest request, CancellationToken ct = default);

    // Lanza KeyNotFoundException si el producto no existe.
    Task<ProductoResponse> ActualizarAsync(int idProducto, ProductoRequest request, CancellationToken ct = default);
}

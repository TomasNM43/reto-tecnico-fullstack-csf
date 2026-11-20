using Movimientos.Service.Application.Dtos;

namespace Movimientos.Service.Application.Interfaces;

// Facade hacia Productos.Service: esconde los detalles de la llamada HTTP entre microservicios.
public interface IProductosFacade
{
    Task<IReadOnlyList<ProductoDto>> ListarAsync(CancellationToken ct = default);

    // Devuelve null si el producto no existe.
    Task<ProductoDto?> ObtenerAsync(int idProducto, CancellationToken ct = default);
}

using Ventas.Service.Application.Dtos;

namespace Ventas.Service.Application.Interfaces;

// Facade hacia Productos.Service: esconde la comunicación HTTP entre microservicios.
public interface IProductosFacade
{
    // Devuelve null si el producto no existe.
    Task<ProductoDto?> ObtenerAsync(int idProducto, CancellationToken ct = default);
}

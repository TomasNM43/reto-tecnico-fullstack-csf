using Compras.Service.Application.Dtos;

namespace Compras.Service.Application.Interfaces;

// Facade hacia Productos.Service: esconde la comunicación HTTP entre microservicios.
public interface IProductosFacade
{
    // Devuelve null si el producto no existe.
    Task<ProductoDto?> ObtenerAsync(int idProducto, CancellationToken ct = default);

    // Cambia el Costo y el PrecioVenta de un producto, dejando igual el resto de sus datos.
    Task ActualizarCostoYPrecioAsync(int idProducto, decimal costo, decimal precioVenta, CancellationToken ct = default);
}

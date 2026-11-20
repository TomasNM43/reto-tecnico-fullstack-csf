namespace Movimientos.Service.Application.Dtos;

// Datos de un producto tal como los devuelve Productos.Service.
public record ProductoDto(
    int IdProducto,
    string NombreProducto,
    string NroLote,
    decimal Costo,
    decimal PrecioVenta);

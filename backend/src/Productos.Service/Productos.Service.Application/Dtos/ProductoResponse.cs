namespace Productos.Service.Application.Dtos;

public record ProductoResponse(
    int IdProducto,
    string NombreProducto,
    string NroLote,
    DateTime FecRegistro,
    decimal Costo,
    decimal PrecioVenta);

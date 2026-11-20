namespace Movimientos.Service.Application.Dtos;

// Una fila de la tabla del Kardex.
public record KardexItemDto(
    int IdProducto,
    string NombreProducto,
    int StockActual,
    decimal Costo,
    decimal PrecioVenta);

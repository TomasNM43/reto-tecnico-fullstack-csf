namespace Movimientos.Service.Application.Dtos;

// Una fila del modal del Kardex: fecha, tipo y cantidad de un movimiento de un producto.
public record MovimientoProductoDto(
    DateTime FecRegistro,
    int IdTipoMovimiento,
    string TipoMovimiento,
    int Cantidad);

namespace Movimientos.Service.Application.Dtos;

public record MovimientoResponse(
    int IdMovimientoCab,
    DateTime FecRegistro,
    int IdTipoMovimiento,
    string TipoMovimiento,
    int IdDocumentoOrigen);

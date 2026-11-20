using Movimientos.Service.Domain.Entities;

namespace Movimientos.Service.Application.Dtos;

// Un movimiento "aplanado" (cabecera + detalle) para poder sumar y restar fácilmente.
public record MovimientoPlano(
    int IdProducto,
    TipoMovimiento Tipo,
    int Cantidad,
    DateTime FecRegistro);

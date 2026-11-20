using Movimientos.Service.Application.Dtos;

namespace Movimientos.Service.Application.Interfaces;

public interface IMovimientoService
{
    // Registra un movimiento (cabecera + detalle). Lo llaman Compras y Ventas.
    Task<MovimientoResponse> RegistrarAsync(RegistrarMovimientoRequest request, CancellationToken ct = default);
}

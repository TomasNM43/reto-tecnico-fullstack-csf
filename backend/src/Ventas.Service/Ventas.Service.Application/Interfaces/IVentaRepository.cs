using Ventas.Service.Domain.Entities;

namespace Ventas.Service.Application.Interfaces;

public interface IVentaRepository
{
    // Guarda la cabecera y su detalle en una sola transacción. Rellena los Id generados.
    Task AgregarAsync(VentaCab venta, CancellationToken ct = default);

    // Borra una venta y su detalle. Sirve para deshacer si algo falla después de guardarla.
    Task EliminarAsync(int idVentaCab, CancellationToken ct = default);

    Task<IReadOnlyList<VentaCab>> ListarAsync(CancellationToken ct = default);
}

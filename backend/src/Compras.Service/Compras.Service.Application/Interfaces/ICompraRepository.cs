using Compras.Service.Domain.Entities;

namespace Compras.Service.Application.Interfaces;

public interface ICompraRepository
{
    // Guarda la cabecera y su detalle en una sola transacción. Rellena los Id generados.
    Task AgregarAsync(CompraCab compra, CancellationToken ct = default);

    // Borra una compra y su detalle. Sirve para deshacer si algo falla después de guardarla.
    Task EliminarAsync(int idCompraCab, CancellationToken ct = default);

    Task<IReadOnlyList<CompraCab>> ListarAsync(CancellationToken ct = default);
}

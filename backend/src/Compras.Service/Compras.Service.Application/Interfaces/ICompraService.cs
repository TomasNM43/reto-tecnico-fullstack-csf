using Compras.Service.Application.Dtos;

namespace Compras.Service.Application.Interfaces;

public interface ICompraService
{
    Task<CompraResponse> RegistrarAsync(RegistrarCompraRequest request, CancellationToken ct = default);

    Task<IReadOnlyList<CompraResponse>> ListarAsync(CancellationToken ct = default);
}

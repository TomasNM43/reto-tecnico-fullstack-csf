using Ventas.Service.Application.Dtos;

namespace Ventas.Service.Application.Interfaces;

public interface IVentaService
{
    Task<VentaResponse> RegistrarAsync(RegistrarVentaRequest request, CancellationToken ct = default);

    Task<IReadOnlyList<VentaResponse>> ListarAsync(CancellationToken ct = default);
}

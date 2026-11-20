using System.Net.Http.Json;
using Compras.Service.Application.Dtos;
using Compras.Service.Application.Interfaces;

namespace Compras.Service.Infrastructure.Clients;

// Implementación de la Facade: habla por HTTP con Movimientos.Service.
public class MovimientosHttpFacade : IMovimientosFacade
{
    private const int TipoEntrada = 1;

    private readonly HttpClient _http;

    public MovimientosHttpFacade(HttpClient http) => _http = http;

    public async Task RegistrarEntradaAsync(int idCompraCab, IReadOnlyList<MovimientoItemDto> items, CancellationToken ct = default)
    {
        var cuerpo = new
        {
            idTipoMovimiento = TipoEntrada,
            idDocumentoOrigen = idCompraCab,
            detalles = items.Select(i => new { idProducto = i.IdProducto, cantidad = i.Cantidad })
        };

        var respuesta = await _http.PostAsJsonAsync("api/movimientos", cuerpo, ct);
        respuesta.EnsureSuccessStatusCode();
    }
}

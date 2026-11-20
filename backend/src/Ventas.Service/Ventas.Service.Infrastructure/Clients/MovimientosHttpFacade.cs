using System.Net.Http.Json;
using Ventas.Service.Application.Dtos;
using Ventas.Service.Application.Interfaces;

namespace Ventas.Service.Infrastructure.Clients;

// Implementación de la Facade: habla por HTTP con Movimientos.Service.
public class MovimientosHttpFacade : IMovimientosFacade
{
    private const int TipoSalida = 2;

    private readonly HttpClient _http;

    public MovimientosHttpFacade(HttpClient http) => _http = http;

    public async Task<int> ObtenerStockAsync(int idProducto, CancellationToken ct = default)
        => await _http.GetFromJsonAsync<int>($"api/kardex/{idProducto}/stock", ct);

    public async Task RegistrarSalidaAsync(int idVentaCab, IReadOnlyList<MovimientoItemDto> items, CancellationToken ct = default)
    {
        var cuerpo = new
        {
            idTipoMovimiento = TipoSalida,
            idDocumentoOrigen = idVentaCab,
            detalles = items.Select(i => new { idProducto = i.IdProducto, cantidad = i.Cantidad })
        };

        var respuesta = await _http.PostAsJsonAsync("api/movimientos", cuerpo, ct);
        respuesta.EnsureSuccessStatusCode();
    }
}

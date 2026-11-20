using System.Net;
using System.Net.Http.Json;
using Movimientos.Service.Application.Dtos;
using Movimientos.Service.Application.Interfaces;

namespace Movimientos.Service.Infrastructure.Clients;

// Implementación de la Facade: habla por HTTP con Productos.Service.
public class ProductosHttpFacade : IProductosFacade
{
    private readonly HttpClient _http;

    public ProductosHttpFacade(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<ProductoDto>> ListarAsync(CancellationToken ct = default)
    {
        var productos = await _http.GetFromJsonAsync<List<ProductoDto>>("api/productos", ct);
        return productos ?? new List<ProductoDto>();
    }

    public async Task<ProductoDto?> ObtenerAsync(int idProducto, CancellationToken ct = default)
    {
        var respuesta = await _http.GetAsync($"api/productos/{idProducto}", ct);

        if (respuesta.StatusCode == HttpStatusCode.NotFound)
            return null;

        respuesta.EnsureSuccessStatusCode();
        return await respuesta.Content.ReadFromJsonAsync<ProductoDto>(cancellationToken: ct);
    }
}

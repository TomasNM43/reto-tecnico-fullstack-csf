using System.Net;
using System.Net.Http.Json;
using Compras.Service.Application.Dtos;
using Compras.Service.Application.Interfaces;

namespace Compras.Service.Infrastructure.Clients;

// Implementación de la Facade: habla por HTTP con Productos.Service.
public class ProductosHttpFacade : IProductosFacade
{
    private readonly HttpClient _http;

    public ProductosHttpFacade(HttpClient http) => _http = http;

    public async Task<ProductoDto?> ObtenerAsync(int idProducto, CancellationToken ct = default)
    {
        var respuesta = await _http.GetAsync($"api/productos/{idProducto}", ct);

        if (respuesta.StatusCode == HttpStatusCode.NotFound)
            return null;

        respuesta.EnsureSuccessStatusCode();
        return await respuesta.Content.ReadFromJsonAsync<ProductoDto>(cancellationToken: ct);
    }

    public async Task ActualizarCostoYPrecioAsync(int idProducto, decimal costo, decimal precioVenta, CancellationToken ct = default)
    {
        var actual = await ObtenerAsync(idProducto, ct)
            ?? throw new KeyNotFoundException($"No existe el producto con id {idProducto}.");

        // Productos.Service pide el producto completo, así que se reenvían los datos que no cambian.
        var cuerpo = new
        {
            nombreProducto = actual.NombreProducto,
            nroLote = actual.NroLote,
            costo,
            precioVenta
        };

        var respuesta = await _http.PutAsJsonAsync($"api/productos/{idProducto}", cuerpo, ct);
        respuesta.EnsureSuccessStatusCode();
    }
}

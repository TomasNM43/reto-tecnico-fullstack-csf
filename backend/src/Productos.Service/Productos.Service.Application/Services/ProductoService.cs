using Productos.Service.Application.Dtos;
using Productos.Service.Application.Interfaces;
using Productos.Service.Domain.Entities;

namespace Productos.Service.Application.Services;

public class ProductoService : IProductoService
{
    private readonly IProductoRepository _productos;

    public ProductoService(IProductoRepository productos) => _productos = productos;

    public async Task<IReadOnlyList<ProductoResponse>> ListarAsync(CancellationToken ct = default)
    {
        var lista = await _productos.ListarAsync(ct);
        return lista.Select(ToResponse).ToList();
    }

    public async Task<ProductoResponse> ObtenerAsync(int idProducto, CancellationToken ct = default)
        => ToResponse(await BuscarAsync(idProducto, ct));

    public async Task<ProductoResponse> RegistrarAsync(ProductoRequest request, CancellationToken ct = default)
    {
        var producto = new Producto
        {
            NombreProducto = request.NombreProducto.Trim(),
            NroLote = request.NroLote.Trim(),
            Costo = request.Costo,
            PrecioVenta = request.PrecioVenta
        };

        await _productos.AgregarAsync(producto, ct);
        return ToResponse(producto);
    }

    public async Task<ProductoResponse> ActualizarAsync(int idProducto, ProductoRequest request, CancellationToken ct = default)
    {
        var producto = await BuscarAsync(idProducto, ct);

        producto.NombreProducto = request.NombreProducto.Trim();
        producto.NroLote = request.NroLote.Trim();
        producto.Costo = request.Costo;
        producto.PrecioVenta = request.PrecioVenta;

        await _productos.ActualizarAsync(producto, ct);
        return ToResponse(producto);
    }

    private async Task<Producto> BuscarAsync(int idProducto, CancellationToken ct)
        => await _productos.ObtenerPorIdAsync(idProducto, ct)
           ?? throw new KeyNotFoundException($"No existe el producto con id {idProducto}.");

    private static ProductoResponse ToResponse(Producto p)
        => new(p.IdProducto, p.NombreProducto, p.NroLote, p.FecRegistro, p.Costo, p.PrecioVenta);
}

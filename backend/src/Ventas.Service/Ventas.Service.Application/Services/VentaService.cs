using BuildingBlocks.Errors;
using Ventas.Service.Application.Configuration;
using Ventas.Service.Application.Dtos;
using Ventas.Service.Application.Interfaces;
using Ventas.Service.Domain.Entities;

namespace Ventas.Service.Application.Services;

public class VentaService : IVentaService
{
    private readonly IVentaRepository _ventas;
    private readonly IProductosFacade _productos;
    private readonly IMovimientosFacade _movimientos;
    private readonly ConfiguracionNegocio _negocio;

    public VentaService(
        IVentaRepository ventas,
        IProductosFacade productos,
        IMovimientosFacade movimientos,
        ConfiguracionNegocio negocio)
    {
        _ventas = ventas;
        _productos = productos;
        _movimientos = movimientos;
        _negocio = negocio;
    }

    public async Task<VentaResponse> RegistrarAsync(RegistrarVentaRequest request, CancellationToken ct = default)
    {
        var productos = await ObtenerProductosAsync(request.Detalles.Select(d => d.IdProducto), ct);
        await ValidarStockAsync(request.Detalles, productos, ct);

        // El precio de cada línea es el precio de venta actual del producto.
        var venta = new VentaCab
        {
            Detalles = request.Detalles.Select(d => CrearDetalle(d, productos[d.IdProducto].PrecioVenta)).ToList()
        };
        venta.SubTotal = venta.Detalles.Sum(d => d.SubTotal);
        venta.Igv = venta.Detalles.Sum(d => d.Igv);
        venta.Total = venta.Detalles.Sum(d => d.Total);

        await _ventas.AgregarAsync(venta, ct);

        try
        {
            // El movimiento se guarda completo o no se guarda; si falla, se borra la venta.
            var items = venta.Detalles.Select(d => new MovimientoItemDto(d.IdProducto, d.Cantidad)).ToList();
            await _movimientos.RegistrarSalidaAsync(venta.IdVentaCab, items, ct);
        }
        catch
        {
            await DeshacerAsync(venta.IdVentaCab);
            throw;
        }

        return ToResponse(venta);
    }

    public async Task<IReadOnlyList<VentaResponse>> ListarAsync(CancellationToken ct = default)
    {
        var ventas = await _ventas.ListarAsync(ct);
        return ventas.Select(ToResponse).ToList();
    }

    private VentaDet CrearDetalle(VentaItemRequest item, decimal precioVenta)
    {
        var subTotal = Redondear(item.Cantidad * precioVenta);
        var igv = Redondear(subTotal * _negocio.IgvRate);

        return new VentaDet
        {
            IdProducto = item.IdProducto,
            Cantidad = item.Cantidad,
            Precio = precioVenta,
            SubTotal = subTotal,
            Igv = igv,
            Total = subTotal + igv
        };
    }

    private async Task<Dictionary<int, ProductoDto>> ObtenerProductosAsync(IEnumerable<int> ids, CancellationToken ct)
    {
        var productos = new Dictionary<int, ProductoDto>();

        foreach (var id in ids.Distinct())
        {
            productos[id] = await _productos.ObtenerAsync(id, ct)
                ?? throw new ReglaNegocioException($"El producto con id {id} no existe.");
        }

        return productos;
    }

    // La cantidad pedida de cada producto (sumando sus líneas) no puede superar el stock disponible.
    private async Task ValidarStockAsync(
        IEnumerable<VentaItemRequest> detalles, Dictionary<int, ProductoDto> productos, CancellationToken ct)
    {
        var errores = new List<string>();

        foreach (var grupo in detalles.GroupBy(d => d.IdProducto))
        {
            var solicitado = grupo.Sum(d => d.Cantidad);
            var stock = await _movimientos.ObtenerStockAsync(grupo.Key, ct);

            if (solicitado <= stock) continue;

            var nombre = productos[grupo.Key].NombreProducto;
            errores.Add(stock <= 0
                ? $"No hay stock de \"{nombre}\"."
                : $"Stock insuficiente de \"{nombre}\": se piden {solicitado} y hay {stock}.");
        }

        if (errores.Count > 0)
            throw new ReglaNegocioException(string.Join(" ", errores));
    }

    // Borra la venta. Si falla se ignora, para que el usuario vea el error original.
    private async Task DeshacerAsync(int idVentaCab)
    {
        try
        {
            await _ventas.EliminarAsync(idVentaCab);
        }
        catch (Exception)
        {
            // Nada más que hacer.
        }
    }

    // Los importes se redondean a 2 decimales (S/ 0.005 sube a S/ 0.01).
    private static decimal Redondear(decimal valor) => Math.Round(valor, 2, MidpointRounding.AwayFromZero);

    private static VentaResponse ToResponse(VentaCab v)
        => new(
            v.IdVentaCab,
            v.FecRegistro,
            v.SubTotal,
            v.Igv,
            v.Total,
            v.Detalles
                .Select(d => new VentaDetalleResponse(d.IdVentaDet, d.IdProducto, d.Cantidad, d.Precio, d.SubTotal, d.Igv, d.Total))
                .ToList());
}

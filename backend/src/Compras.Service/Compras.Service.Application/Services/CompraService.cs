using BuildingBlocks.Errors;
using Compras.Service.Application.Configuration;
using Compras.Service.Application.Dtos;
using Compras.Service.Application.Interfaces;
using Compras.Service.Domain.Entities;

namespace Compras.Service.Application.Services;

public class CompraService : ICompraService
{
    private readonly ICompraRepository _compras;
    private readonly IProductosFacade _productos;
    private readonly IMovimientosFacade _movimientos;
    private readonly ConfiguracionNegocio _negocio;

    public CompraService(
        ICompraRepository compras,
        IProductosFacade productos,
        IMovimientosFacade movimientos,
        ConfiguracionNegocio negocio)
    {
        _compras = compras;
        _productos = productos;
        _movimientos = movimientos;
        _negocio = negocio;
    }

    public async Task<CompraResponse> RegistrarAsync(RegistrarCompraRequest request, CancellationToken ct = default)
    {
        // Se leen los productos antes de guardar nada: si alguno no existe, la compra ni se crea.
        var originales = await ObtenerProductosAsync(request.Detalles.Select(d => d.IdProducto), ct);

        var compra = new CompraCab { Detalles = request.Detalles.Select(CrearDetalle).ToList() };
        compra.SubTotal = compra.Detalles.Sum(d => d.SubTotal);
        compra.Igv = compra.Detalles.Sum(d => d.Igv);
        compra.Total = compra.Detalles.Sum(d => d.Total);

        await _compras.AgregarAsync(compra, ct);

        var actualizados = new List<ProductoDto>();
        try
        {
            // Cada producto toma como costo su último precio de compra, y su precio de venta es costo × factor.
            foreach (var grupo in request.Detalles.GroupBy(d => d.IdProducto))
            {
                var costo = grupo.Last().Precio;
                var precioVenta = Redondear(costo * _negocio.FactorPrecioVenta);

                await _productos.ActualizarCostoYPrecioAsync(grupo.Key, costo, precioVenta, ct);
                actualizados.Add(originales[grupo.Key]);
            }

            // El movimiento va al final: se guarda completo o no se guarda, así nunca queda uno huérfano.
            var items = compra.Detalles.Select(d => new MovimientoItemDto(d.IdProducto, d.Cantidad)).ToList();
            await _movimientos.RegistrarEntradaAsync(compra.IdCompraCab, items, ct);
        }
        catch
        {
            await DeshacerAsync(compra.IdCompraCab, actualizados);
            throw;
        }

        return ToResponse(compra);
    }

    public async Task<IReadOnlyList<CompraResponse>> ListarAsync(CancellationToken ct = default)
    {
        var compras = await _compras.ListarAsync(ct);
        return compras.Select(ToResponse).ToList();
    }

    private CompraDet CrearDetalle(CompraItemRequest item)
    {
        var subTotal = Redondear(item.Cantidad * item.Precio);
        var igv = Redondear(subTotal * _negocio.IgvRate);

        return new CompraDet
        {
            IdProducto = item.IdProducto,
            Cantidad = item.Cantidad,
            Precio = item.Precio,
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

    // Devuelve los productos a su costo y precio anteriores y borra la compra.
    // Si algo de esto falla se ignora, para que el usuario vea el error original y no el del deshacer.
    private async Task DeshacerAsync(int idCompraCab, IEnumerable<ProductoDto> originales)
    {
        foreach (var original in originales)
        {
            try
            {
                await _productos.ActualizarCostoYPrecioAsync(original.IdProducto, original.Costo, original.PrecioVenta);
            }
            catch (Exception)
            {
                // Se sigue con el resto.
            }
        }

        try
        {
            await _compras.EliminarAsync(idCompraCab);
        }
        catch (Exception)
        {
            // Nada más que hacer.
        }
    }

    // Los importes se redondean a 2 decimales (S/ 0.005 sube a S/ 0.01).
    private static decimal Redondear(decimal valor) => Math.Round(valor, 2, MidpointRounding.AwayFromZero);

    private static CompraResponse ToResponse(CompraCab c)
        => new(
            c.IdCompraCab,
            c.FecRegistro,
            c.SubTotal,
            c.Igv,
            c.Total,
            c.Detalles
                .Select(d => new CompraDetalleResponse(d.IdCompraDet, d.IdProducto, d.Cantidad, d.Precio, d.SubTotal, d.Igv, d.Total))
                .ToList());
}

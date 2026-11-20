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

    public Task<CompraResponse> RegistrarAsync(RegistrarCompraRequest request, CancellationToken ct = default)
    {
        // TODO(USUARIO): registrar la compra completa.
        // REGLA: 1) calcular SubTotal, Igv y Total de cada detalle y de la cabecera (IGV = SubTotal × _negocio.IgvRate);
        //        2) actualizar Costo y PrecioVenta de cada producto (Costo = Precio de compra, PrecioVenta = Costo × _negocio.FactorPrecioVenta);
        //        3) generar el movimiento de Entrada. Todo debe quedar consistente: si algo falla, no puede quedar una compra sin su movimiento.
        // PISTA: arma un CompraCab con sus CompraDet y guárdalo con _compras.AgregarAsync (te devuelve los Id generados);
        //        luego usa _productos.ActualizarCostoYPrecioAsync y _movimientos.RegistrarEntradaAsync(idCompra, items);
        //        si una llamada falla, deshaz la compra con _compras.EliminarAsync(idCompra) y vuelve a lanzar el error.
        //        Si un producto no existe, lanza ReglaNegocioException (BuildingBlocks.Errors) para responder 400.
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<CompraResponse>> ListarAsync(CancellationToken ct = default)
    {
        var compras = await _compras.ListarAsync(ct);
        return compras.Select(ToResponse).ToList();
    }

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

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

    public Task<VentaResponse> RegistrarAsync(RegistrarVentaRequest request, CancellationToken ct = default)
    {
        // TODO(USUARIO): registrar la venta completa.
        // REGLA: 1) validar que la cantidad de cada producto no supere su stock disponible (si no alcanza, responder con un mensaje claro);
        //        2) calcular SubTotal, Igv y Total de cada detalle y de la cabecera (IGV = SubTotal × _negocio.IgvRate);
        //        3) generar el movimiento de Salida. Todo debe quedar consistente: si algo falla, no puede quedar una venta sin su movimiento.
        // PISTA: _productos.ObtenerAsync(id) te da el PrecioVenta y _movimientos.ObtenerStockAsync(id) el stock actual;
        //        si el stock no alcanza o el producto no existe, lanza ReglaNegocioException (BuildingBlocks.Errors) para responder 400;
        //        arma un VentaCab con sus VentaDet y guárdalo con _ventas.AgregarAsync (te devuelve los Id generados);
        //        luego llama a _movimientos.RegistrarSalidaAsync(idVenta, items); si falla, deshaz con _ventas.EliminarAsync(idVenta) y vuelve a lanzar el error.
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<VentaResponse>> ListarAsync(CancellationToken ct = default)
    {
        var ventas = await _ventas.ListarAsync(ct);
        return ventas.Select(ToResponse).ToList();
    }

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

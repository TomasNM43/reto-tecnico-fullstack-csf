using Movimientos.Service.Application.Dtos;
using Movimientos.Service.Application.Interfaces;
using Movimientos.Service.Domain.Entities;

namespace Movimientos.Service.Application.Services;

public class MovimientoService : IMovimientoService
{
    private readonly IMovimientoRepository _movimientos;
    private readonly IProductosFacade _productos;

    public MovimientoService(IMovimientoRepository movimientos, IProductosFacade productos)
    {
        _movimientos = movimientos;
        _productos = productos;
    }

    public async Task<MovimientoResponse> RegistrarAsync(RegistrarMovimientoRequest request, CancellationToken ct = default)
    {
        // Antes de guardar, se comprueba que cada producto exista en Productos.Service.
        foreach (var idProducto in request.Detalles.Select(d => d.IdProducto).Distinct())
        {
            if (await _productos.ObtenerAsync(idProducto, ct) is null)
                throw new ArgumentException($"El producto con id {idProducto} no existe.");
        }

        var movimiento = new MovimientoCab
        {
            IdTipoMovimiento = request.IdTipoMovimiento,
            IdDocumentoOrigen = request.IdDocumentoOrigen,
            Detalles = request.Detalles
                .Select(d => new MovimientoDet { IdProducto = d.IdProducto, Cantidad = d.Cantidad })
                .ToList()
        };

        await _movimientos.AgregarAsync(movimiento, ct);

        return new MovimientoResponse(
            movimiento.IdMovimientoCab,
            movimiento.FecRegistro,
            (int)movimiento.IdTipoMovimiento,
            movimiento.IdTipoMovimiento.ToString(),
            movimiento.IdDocumentoOrigen);
    }
}

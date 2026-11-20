using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movimientos.Service.Application.Dtos;
using Movimientos.Service.Application.Interfaces;

namespace Movimientos.Service.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/kardex")]
public class KardexController : ControllerBase
{
    private readonly IKardexService _kardex;
    private readonly IStockService _stock;

    public KardexController(IKardexService kardex, IStockService stock)
    {
        _kardex = kardex;
        _stock = stock;
    }

    // Tabla principal del Kardex.
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<KardexItemDto>>> Listar(CancellationToken ct)
        => Ok(await _kardex.ListarKardexAsync(ct));

    // Movimientos de un producto (modal de cada fila).
    [HttpGet("{idProducto:int}/movimientos")]
    public async Task<ActionResult<IReadOnlyList<MovimientoProductoDto>>> ListarMovimientos(int idProducto, CancellationToken ct)
        => Ok(await _kardex.ListarMovimientosAsync(idProducto, ct));

    // Stock actual de un producto (lo usan la vista de Venta y el servicio de Ventas).
    [HttpGet("{idProducto:int}/stock")]
    public async Task<ActionResult<int>> ObtenerStock(int idProducto, CancellationToken ct)
        => Ok(await _stock.ObtenerStockAsync(idProducto, ct));
}

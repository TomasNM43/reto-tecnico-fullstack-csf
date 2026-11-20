using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movimientos.Service.Application.Dtos;
using Movimientos.Service.Application.Interfaces;

namespace Movimientos.Service.Api.Controllers;

// Endpoint interno: lo usan Compras.Service y Ventas.Service para registrar sus movimientos.
[ApiController]
[Authorize]
[Route("api/movimientos")]
public class MovimientosController : ControllerBase
{
    private readonly IMovimientoService _movimientos;

    public MovimientosController(IMovimientoService movimientos) => _movimientos = movimientos;

    [HttpPost]
    [ProducesResponseType(typeof(MovimientoResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<MovimientoResponse>> Registrar([FromBody] RegistrarMovimientoRequest request, CancellationToken ct)
    {
        var creado = await _movimientos.RegistrarAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, creado);
    }
}

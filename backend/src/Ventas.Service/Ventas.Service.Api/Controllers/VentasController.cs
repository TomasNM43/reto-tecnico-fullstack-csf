using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ventas.Service.Application.Dtos;
using Ventas.Service.Application.Interfaces;

namespace Ventas.Service.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/ventas")]
public class VentasController : ControllerBase
{
    private readonly IVentaService _ventas;

    public VentasController(IVentaService ventas) => _ventas = ventas;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<VentaResponse>>> Listar(CancellationToken ct)
        => Ok(await _ventas.ListarAsync(ct));

    [HttpPost]
    [ProducesResponseType(typeof(VentaResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<VentaResponse>> Registrar([FromBody] RegistrarVentaRequest request, CancellationToken ct)
    {
        var creada = await _ventas.RegistrarAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, creada);
    }
}

using Compras.Service.Application.Dtos;
using Compras.Service.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Compras.Service.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/compras")]
public class ComprasController : ControllerBase
{
    private readonly ICompraService _compras;

    public ComprasController(ICompraService compras) => _compras = compras;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CompraResponse>>> Listar(CancellationToken ct)
        => Ok(await _compras.ListarAsync(ct));

    [HttpPost]
    [ProducesResponseType(typeof(CompraResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<CompraResponse>> Registrar([FromBody] RegistrarCompraRequest request, CancellationToken ct)
    {
        var creada = await _compras.RegistrarAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, creada);
    }
}

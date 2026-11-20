using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Productos.Service.Application.Dtos;
using Productos.Service.Application.Interfaces;

namespace Productos.Service.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/productos")]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _productos;

    public ProductosController(IProductoService productos) => _productos = productos;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductoResponse>>> Listar(CancellationToken ct)
        => Ok(await _productos.ListarAsync(ct));

    [HttpGet("{idProducto:int}")]
    public async Task<ActionResult<ProductoResponse>> Obtener(int idProducto, CancellationToken ct)
        => Ok(await _productos.ObtenerAsync(idProducto, ct));

    [HttpPost]
    public async Task<ActionResult<ProductoResponse>> Registrar([FromBody] ProductoRequest request, CancellationToken ct)
    {
        var creado = await _productos.RegistrarAsync(request, ct);
        return CreatedAtAction(nameof(Obtener), new { idProducto = creado.IdProducto }, creado);
    }

    [HttpPut("{idProducto:int}")]
    public async Task<ActionResult<ProductoResponse>> Actualizar(int idProducto, [FromBody] ProductoRequest request, CancellationToken ct)
        => Ok(await _productos.ActualizarAsync(idProducto, request, ct));
}

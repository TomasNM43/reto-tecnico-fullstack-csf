using Auth.Service.Application.Dtos;
using Auth.Service.Application.Interfaces;
using BuildingBlocks.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Auth.Service.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ILoginUseCase _login;

    public AuthController(ILoginUseCase login) => _login = login;

    // Política de rate limit estricta para frenar intentos de fuerza bruta.
    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitOptions.LoginPolicy)]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var respuesta = await _login.EjecutarAsync(request, ct);

        if (respuesta is null)
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Credenciales inválidas",
                Detail = "Usuario o contraseña incorrectos."
            });

        return Ok(respuesta);
    }
}

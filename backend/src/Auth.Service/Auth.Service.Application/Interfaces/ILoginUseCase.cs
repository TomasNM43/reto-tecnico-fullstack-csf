using Auth.Service.Application.Dtos;

namespace Auth.Service.Application.Interfaces;

public interface ILoginUseCase
{
    // Devuelve null si las credenciales no son válidas.
    Task<LoginResponse?> EjecutarAsync(LoginRequest request, CancellationToken ct = default);
}

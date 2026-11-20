using Auth.Service.Domain.Entities;

namespace Auth.Service.Application.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorUsernameAsync(string username, CancellationToken ct = default);
}

using Auth.Service.Application.Interfaces;
using Auth.Service.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth.Service.Infrastructure.Persistence;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AuthDbContext _db;

    public UsuarioRepository(AuthDbContext db) => _db = db;

    public Task<Usuario?> ObtenerPorUsernameAsync(string username, CancellationToken ct = default)
        => _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username, ct);
}

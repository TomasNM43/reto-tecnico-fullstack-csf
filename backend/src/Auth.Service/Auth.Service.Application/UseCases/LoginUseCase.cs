using Auth.Service.Application.Dtos;
using Auth.Service.Application.Interfaces;

namespace Auth.Service.Application.UseCases;

public class LoginUseCase : ILoginUseCase
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenGenerator _tokens;

    public LoginUseCase(IUsuarioRepository usuarios, IPasswordHasher hasher, ITokenGenerator tokens)
    {
        _usuarios = usuarios;
        _hasher = hasher;
        _tokens = tokens;
    }

    public async Task<LoginResponse?> EjecutarAsync(LoginRequest request, CancellationToken ct = default)
    {
        var usuario = await _usuarios.ObtenerPorUsernameAsync(request.Username.Trim(), ct);

        // Mismo resultado si el usuario no existe o la clave es incorrecta, para no dar pistas.
        if (usuario is null || !_hasher.Verificar(request.Password, usuario.PasswordHash))
            return null;

        var (token, expira) = _tokens.Generar(usuario);
        return new LoginResponse(token, expira, usuario.Username, usuario.Rol);
    }
}

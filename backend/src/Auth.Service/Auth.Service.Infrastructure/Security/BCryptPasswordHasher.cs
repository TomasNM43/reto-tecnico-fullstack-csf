using Auth.Service.Application.Interfaces;

namespace Auth.Service.Infrastructure.Security;

public class BCryptPasswordHasher : IPasswordHasher
{
    public bool Verificar(string password, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            return false; // hash mal formado en la BD: se trata como credencial inválida
        }
    }
}

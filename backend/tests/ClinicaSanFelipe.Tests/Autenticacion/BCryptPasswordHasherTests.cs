using Auth.Service.Infrastructure.Security;

namespace ClinicaSanFelipe.Tests.Autenticacion;

public class BCryptPasswordHasherTests
{
    private readonly BCryptPasswordHasher _hasher = new();

    [Fact]
    public void Verifica_una_clave_contra_su_hash()
    {
        var hash = BCrypt.Net.BCrypt.HashPassword("Admin123*");

        Assert.True(_hasher.Verificar("Admin123*", hash));
        Assert.False(_hasher.Verificar("otra-clave", hash));
    }

    [Fact]
    public void Un_hash_mal_formado_se_trata_como_credencial_invalida()
    {
        Assert.False(_hasher.Verificar("Admin123*", "esto-no-es-un-hash"));
    }
}

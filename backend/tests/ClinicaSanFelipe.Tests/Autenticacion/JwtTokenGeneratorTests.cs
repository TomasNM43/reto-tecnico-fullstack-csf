using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Service.Domain.Entities;
using Auth.Service.Infrastructure.Security;
using BuildingBlocks.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ClinicaSanFelipe.Tests.Autenticacion;

public class JwtTokenGeneratorTests
{
    private const string Clave = "clave-de-pruebas-con-mas-de-32-caracteres-123456";

    private static JwtTokenGenerator CrearGenerador(string clave = Clave)
        => new(Options.Create(new JwtOptions { Key = clave, Issuer = "emisor", Audience = "audiencia", ExpirationMinutes = 30 }));

    private static Usuario Admin() => new() { Id = 7, Username = "admin", Rol = "Admin" };

    [Fact]
    public void El_token_expira_a_los_30_minutos()
    {
        var antes = DateTime.UtcNow;

        var (_, expira) = CrearGenerador().Generar(Admin());

        var despues = DateTime.UtcNow;
        Assert.InRange(expira, antes.AddMinutes(30), despues.AddMinutes(30));
    }

    [Fact]
    public void El_token_lleva_el_usuario_y_el_rol()
    {
        var (token, _) = CrearGenerador().Generar(Admin());

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal("7", jwt.Subject);
        Assert.Equal("admin", jwt.Claims.Single(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value);
        Assert.Equal("Admin", jwt.Claims.Single(c => c.Type == ClaimTypes.Role).Value);
        Assert.Equal("emisor", jwt.Issuer);
    }

    [Fact]
    public void El_token_solo_se_valida_con_la_clave_con_la_que_se_firmo()
    {
        var (token, _) = CrearGenerador().Generar(Admin());
        var manejador = new JwtSecurityTokenHandler();

        TokenValidationParameters ParametrosCon(string clave) => new()
        {
            ValidIssuer = "emisor",
            ValidAudience = "audiencia",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave)),
            ClockSkew = TimeSpan.Zero
        };

        manejador.ValidateToken(token, ParametrosCon(Clave), out var validado);
        Assert.NotNull(validado);

        Assert.ThrowsAny<SecurityTokenException>(
            () => manejador.ValidateToken(token, ParametrosCon("otra-clave-distinta-con-mas-de-32-caracteres-999"), out _));
    }
}

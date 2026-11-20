using Auth.Service.Application.Dtos;
using Auth.Service.Application.Interfaces;
using Auth.Service.Application.UseCases;
using Auth.Service.Domain.Entities;
using NSubstitute;

namespace ClinicaSanFelipe.Tests.Autenticacion;

public class LoginUseCaseTests
{
    private readonly IUsuarioRepository _usuarios = Substitute.For<IUsuarioRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenGenerator _tokens = Substitute.For<ITokenGenerator>();

    private LoginUseCase CrearCasoDeUso() => new(_usuarios, _hasher, _tokens);

    private static Usuario Admin() => new() { Id = 1, Username = "admin", PasswordHash = "hash", Rol = "Admin" };

    [Fact]
    public async Task Con_credenciales_correctas_devuelve_el_token_y_los_datos_del_usuario()
    {
        var expira = DateTime.UtcNow.AddMinutes(30);
        _usuarios.ObtenerPorUsernameAsync("admin", Arg.Any<CancellationToken>()).Returns(Admin());
        _hasher.Verificar("Admin123*", "hash").Returns(true);
        _tokens.Generar(Arg.Any<Usuario>()).Returns(("jwt-de-prueba", expira));

        var respuesta = await CrearCasoDeUso().EjecutarAsync(new LoginRequest { Username = "admin", Password = "Admin123*" });

        Assert.NotNull(respuesta);
        Assert.Equal("jwt-de-prueba", respuesta.Token);
        Assert.Equal(expira, respuesta.ExpiraEnUtc);
        Assert.Equal("admin", respuesta.Username);
        Assert.Equal("Admin", respuesta.Rol);
    }

    [Fact]
    public async Task Con_clave_incorrecta_devuelve_null_y_no_genera_token()
    {
        _usuarios.ObtenerPorUsernameAsync("admin", Arg.Any<CancellationToken>()).Returns(Admin());
        _hasher.Verificar(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        var respuesta = await CrearCasoDeUso().EjecutarAsync(new LoginRequest { Username = "admin", Password = "mala" });

        Assert.Null(respuesta);
        _tokens.DidNotReceive().Generar(Arg.Any<Usuario>());
    }

    [Fact]
    public async Task Con_usuario_inexistente_devuelve_null_sin_verificar_ninguna_clave()
    {
        _usuarios.ObtenerPorUsernameAsync("nadie", Arg.Any<CancellationToken>()).Returns((Usuario?)null);

        var respuesta = await CrearCasoDeUso().EjecutarAsync(new LoginRequest { Username = "nadie", Password = "x" });

        Assert.Null(respuesta);
        _hasher.DidNotReceive().Verificar(Arg.Any<string>(), Arg.Any<string>());
    }
}

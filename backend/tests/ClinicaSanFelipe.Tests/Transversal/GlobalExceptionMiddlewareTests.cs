using BuildingBlocks.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClinicaSanFelipe.Tests.Transversal;

public class GlobalExceptionMiddlewareTests
{
    public static TheoryData<Exception, int> Casos => new()
    {
        { new ReglaNegocioException("Stock insuficiente"), 400 },
        { new ArgumentException("Dato invalido"), 400 },
        { new KeyNotFoundException("No existe"), 404 },
        { new InvalidOperationException("detalle-interno-secreto"), 500 },
    };

    private static async Task<(int Estado, string Cuerpo)> EjecutarAsync(Exception excepcion)
    {
        var contexto = new DefaultHttpContext();
        contexto.Response.Body = new MemoryStream();
        contexto.Request.Path = "/api/prueba";

        var middleware = new GlobalExceptionMiddleware(_ => throw excepcion, NullLogger<GlobalExceptionMiddleware>.Instance);
        await middleware.InvokeAsync(contexto);

        contexto.Response.Body.Position = 0;
        var cuerpo = await new StreamReader(contexto.Response.Body).ReadToEndAsync();
        return (contexto.Response.StatusCode, cuerpo);
    }

    [Theory]
    [MemberData(nameof(Casos))]
    public async Task Convierte_cada_excepcion_en_su_codigo_HTTP(Exception excepcion, int estadoEsperado)
    {
        var (estado, cuerpo) = await EjecutarAsync(excepcion);

        Assert.Equal(estadoEsperado, estado);
        Assert.Contains($"\"status\":{estadoEsperado}", cuerpo);
    }

    [Fact]
    public async Task Una_regla_de_negocio_incumplida_muestra_su_mensaje()
    {
        var (_, cuerpo) = await EjecutarAsync(new ReglaNegocioException("Stock insuficiente"));

        Assert.Contains("Stock insuficiente", cuerpo);
    }

    [Fact]
    public async Task Un_error_inesperado_no_filtra_detalles_internos()
    {
        var (_, cuerpo) = await EjecutarAsync(new InvalidOperationException("detalle-interno-secreto"));

        Assert.DoesNotContain("detalle-interno-secreto", cuerpo);
    }
}

using NSubstitute;
using Productos.Service.Application.Dtos;
using Productos.Service.Application.Interfaces;
using Productos.Service.Application.Services;
using Productos.Service.Domain.Entities;

namespace ClinicaSanFelipe.Tests.Catalogo;

public class ProductoServiceTests
{
    private readonly IProductoRepository _repositorio = Substitute.For<IProductoRepository>();

    private ProductoService CrearServicio() => new(_repositorio);

    private static ProductoRequest Request(string nombre = "Gasa", string lote = "L1")
        => new() { NombreProducto = nombre, NroLote = lote, Costo = 1m, PrecioVenta = 1.35m };

    [Fact]
    public async Task Registrar_guarda_el_producto_sin_espacios_sobrantes()
    {
        var respuesta = await CrearServicio().RegistrarAsync(Request("  Gasa estéril  ", " L7 "));

        await _repositorio.Received(1).AgregarAsync(
            Arg.Is<Producto>(p => p.NombreProducto == "Gasa estéril" && p.NroLote == "L7" && p.PrecioVenta == 1.35m),
            Arg.Any<CancellationToken>());
        Assert.Equal("Gasa estéril", respuesta.NombreProducto);
    }

    [Fact]
    public async Task Actualizar_cambia_los_datos_y_los_guarda()
    {
        var existente = new Producto { IdProducto = 5, NombreProducto = "Viejo", NroLote = "L0", Costo = 1m, PrecioVenta = 2m };
        _repositorio.ObtenerPorIdAsync(5, Arg.Any<CancellationToken>()).Returns(existente);

        var respuesta = await CrearServicio().ActualizarAsync(5, new ProductoRequest
        {
            NombreProducto = "Nuevo", NroLote = "L9", Costo = 3m, PrecioVenta = 4.05m
        });

        Assert.Equal("Nuevo", respuesta.NombreProducto);
        Assert.Equal(4.05m, respuesta.PrecioVenta);
        await _repositorio.Received(1).ActualizarAsync(existente, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Actualizar_un_producto_inexistente_lanza_KeyNotFoundException()
    {
        _repositorio.ObtenerPorIdAsync(99, Arg.Any<CancellationToken>()).Returns((Producto?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => CrearServicio().ActualizarAsync(99, Request()));

        await _repositorio.DidNotReceive().ActualizarAsync(Arg.Any<Producto>(), Arg.Any<CancellationToken>());
    }
}

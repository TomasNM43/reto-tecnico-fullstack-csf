using Movimientos.Service.Application.Dtos;
using Movimientos.Service.Application.Interfaces;
using Movimientos.Service.Application.Services;
using Movimientos.Service.Domain.Entities;
using NSubstitute;

namespace ClinicaSanFelipe.Tests.Inventario;

public class MovimientoServiceTests
{
    private readonly IMovimientoRepository _repositorio = Substitute.For<IMovimientoRepository>();
    private readonly IProductosFacade _productos = Substitute.For<IProductosFacade>();

    private MovimientoService CrearServicio() => new(_repositorio, _productos);

    private static ProductoDto Producto(int id) => new(id, $"Producto {id}", "L1", 10m, 13.5m);

    private static RegistrarMovimientoRequest Entrada(params (int idProducto, int cantidad)[] items) => new()
    {
        IdTipoMovimiento = TipoMovimiento.Entrada,
        IdDocumentoOrigen = 10,
        Detalles = items.Select(i => new MovimientoItemRequest { IdProducto = i.idProducto, Cantidad = i.cantidad }).ToList()
    };

    [Fact]
    public async Task Registrar_guarda_la_cabecera_con_todos_sus_detalles()
    {
        _productos.ObtenerAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(ci => Producto(ci.Arg<int>()));
        _repositorio
            .When(r => r.AgregarAsync(Arg.Any<MovimientoCab>(), Arg.Any<CancellationToken>()))
            .Do(ci => ci.Arg<MovimientoCab>().IdMovimientoCab = 5);

        var respuesta = await CrearServicio().RegistrarAsync(Entrada((1, 20), (2, 5)));

        await _repositorio.Received(1).AgregarAsync(
            Arg.Is<MovimientoCab>(m =>
                m.IdTipoMovimiento == TipoMovimiento.Entrada &&
                m.IdDocumentoOrigen == 10 &&
                m.Detalles.Count == 2 &&
                m.Detalles.Any(d => d.IdProducto == 1 && d.Cantidad == 20)),
            Arg.Any<CancellationToken>());
        Assert.Equal(5, respuesta.IdMovimientoCab);
        Assert.Equal("Entrada", respuesta.TipoMovimiento);
    }

    [Fact]
    public async Task Registrar_con_un_producto_inexistente_lanza_ArgumentException_y_no_guarda_nada()
    {
        _productos.ObtenerAsync(1, Arg.Any<CancellationToken>()).Returns(Producto(1));
        _productos.ObtenerAsync(777, Arg.Any<CancellationToken>()).Returns((ProductoDto?)null);

        var error = await Assert.ThrowsAsync<ArgumentException>(() => CrearServicio().RegistrarAsync(Entrada((1, 1), (777, 1))));

        Assert.Contains("777", error.Message);
        await _repositorio.DidNotReceive().AgregarAsync(Arg.Any<MovimientoCab>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Registrar_consulta_una_sola_vez_cada_producto_repetido()
    {
        _productos.ObtenerAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(ci => Producto(ci.Arg<int>()));

        await CrearServicio().RegistrarAsync(Entrada((1, 2), (1, 3)));

        await _productos.Received(1).ObtenerAsync(1, Arg.Any<CancellationToken>());
    }
}

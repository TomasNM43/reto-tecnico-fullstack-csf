using BuildingBlocks.Errors;
using Compras.Service.Application.Configuration;
using Compras.Service.Application.Dtos;
using Compras.Service.Application.Interfaces;
using Compras.Service.Application.Services;
using Compras.Service.Domain.Entities;
using NSubstitute;

namespace ClinicaSanFelipe.Tests.Negocio;

// Reglas de la compra: calcula los importes (IGV sobre el subtotal), actualiza costo y precio de venta
// (precio = costo x 1.35), genera el movimiento de Entrada y no deja una compra sin su movimiento.
public class CompraServiceTests
{
    private readonly ICompraRepository _compras = Substitute.For<ICompraRepository>();
    private readonly IProductosFacade _productos = Substitute.For<IProductosFacade>();
    private readonly IMovimientosFacade _movimientos = Substitute.For<IMovimientosFacade>();

    public CompraServiceTests()
    {
        // El producto 1 existe con costo S/ 10.00 y precio de venta S/ 13.50; el repositorio asigna el Id 7 a la compra.
        _productos.ObtenerAsync(1, Arg.Any<CancellationToken>()).Returns(new ProductoDto(1, "Paracetamol", "L1", 10m, 13.5m));
        _compras
            .When(r => r.AgregarAsync(Arg.Any<CompraCab>(), Arg.Any<CancellationToken>()))
            .Do(ci => ci.Arg<CompraCab>().IdCompraCab = 7);
    }

    private CompraService CrearServicio() => new(_compras, _productos, _movimientos, new ConfiguracionNegocio());

    private static RegistrarCompraRequest Compra(params (int idProducto, int cantidad, decimal precio)[] items) => new()
    {
        Detalles = items.Select(i => new CompraItemRequest { IdProducto = i.idProducto, Cantidad = i.cantidad, Precio = i.precio }).ToList()
    };

    [Fact]
    public async Task Calcula_subtotal_igv_y_total_de_la_cabecera_y_del_detalle()
    {
        var compra = await CrearServicio().RegistrarAsync(Compra((1, 10, 20m)));

        Assert.Equal(200m, compra.SubTotal);
        Assert.Equal(36m, compra.Igv);
        Assert.Equal(236m, compra.Total);

        var detalle = Assert.Single(compra.Detalles);
        Assert.Equal(200m, detalle.SubTotal);
        Assert.Equal(36m, detalle.Igv);
        Assert.Equal(236m, detalle.Total);
    }

    [Fact]
    public async Task Los_totales_de_la_cabecera_son_la_suma_de_las_lineas_redondeadas()
    {
        _productos.ObtenerAsync(2, Arg.Any<CancellationToken>()).Returns(new ProductoDto(2, "Gasa", "L2", 1m, 1.35m));

        // Línea 1: 1 x 0.25 -> subtotal 0.25, IGV 0.045 que se redondea a 0.05.  Línea 2: 3 x 1.10 -> 3.30 y 0.59.
        var compra = await CrearServicio().RegistrarAsync(Compra((1, 1, 0.25m), (2, 3, 1.10m)));

        Assert.Equal(0.05m, compra.Detalles[0].Igv);
        Assert.Equal(3.55m, compra.SubTotal);
        Assert.Equal(0.64m, compra.Igv);
        Assert.Equal(4.19m, compra.Total);
    }

    [Fact]
    public async Task Actualiza_el_costo_y_el_precio_de_venta_del_producto()
    {
        await CrearServicio().RegistrarAsync(Compra((1, 10, 20m)));

        await _productos.Received(1).ActualizarCostoYPrecioAsync(1, 20m, 27m, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Genera_el_movimiento_de_entrada_de_la_compra()
    {
        await CrearServicio().RegistrarAsync(Compra((1, 10, 20m)));

        await _movimientos.Received(1).RegistrarEntradaAsync(
            7,
            Arg.Is<IReadOnlyList<MovimientoItemDto>>(i => i.Count == 1 && i[0].IdProducto == 1 && i[0].Cantidad == 10),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Con_un_producto_inexistente_responde_con_regla_de_negocio_y_no_guarda_nada()
    {
        _productos.ObtenerAsync(99, Arg.Any<CancellationToken>()).Returns((ProductoDto?)null);

        var error = await Assert.ThrowsAsync<ReglaNegocioException>(() => CrearServicio().RegistrarAsync(Compra((99, 1, 5m))));

        Assert.Contains("99", error.Message);
        await _compras.DidNotReceive().AgregarAsync(Arg.Any<CompraCab>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Si_falla_el_movimiento_deshace_la_compra_y_el_costo_del_producto()
    {
        _movimientos
            .RegistrarEntradaAsync(Arg.Any<int>(), Arg.Any<IReadOnlyList<MovimientoItemDto>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new HttpRequestException("Falló Movimientos.Service")));

        await Assert.ThrowsAsync<HttpRequestException>(() => CrearServicio().RegistrarAsync(Compra((1, 10, 20m))));

        await _compras.Received(1).EliminarAsync(7, Arg.Any<CancellationToken>());
        // El producto vuelve a su costo y precio de venta anteriores.
        await _productos.Received(1).ActualizarCostoYPrecioAsync(1, 10m, 13.5m, Arg.Any<CancellationToken>());
    }
}

using BuildingBlocks.Errors;
using NSubstitute;
using Ventas.Service.Application.Configuration;
using Ventas.Service.Application.Dtos;
using Ventas.Service.Application.Interfaces;
using Ventas.Service.Application.Services;
using Ventas.Service.Domain.Entities;

namespace ClinicaSanFelipe.Tests.Negocio;

// Reglas de la venta: no se vende más que el stock, calcula los importes con el precio de venta,
// genera el movimiento de Salida y no deja una venta sin su movimiento.
public class VentaServiceTests
{
    private readonly IVentaRepository _ventas = Substitute.For<IVentaRepository>();
    private readonly IProductosFacade _productos = Substitute.For<IProductosFacade>();
    private readonly IMovimientosFacade _movimientos = Substitute.For<IMovimientosFacade>();

    public VentaServiceTests()
    {
        // El producto 1 se vende a S/ 13.50; el repositorio asigna el Id 9 a la venta.
        _productos.ObtenerAsync(1, Arg.Any<CancellationToken>()).Returns(new ProductoDto(1, "Paracetamol", "L1", 10m, 13.5m));
        _ventas
            .When(r => r.AgregarAsync(Arg.Any<VentaCab>(), Arg.Any<CancellationToken>()))
            .Do(ci => ci.Arg<VentaCab>().IdVentaCab = 9);
    }

    private VentaService CrearServicio(int stock = 10)
    {
        _movimientos.ObtenerStockAsync(1, Arg.Any<CancellationToken>()).Returns(stock);
        return new VentaService(_ventas, _productos, _movimientos, new ConfiguracionNegocio());
    }

    private static RegistrarVentaRequest Venta(params (int idProducto, int cantidad)[] items) => new()
    {
        Detalles = items.Select(i => new VentaItemRequest { IdProducto = i.idProducto, Cantidad = i.cantidad }).ToList()
    };

    [Fact]
    public async Task No_permite_vender_mas_que_el_stock_y_no_guarda_nada()
    {
        var servicio = CrearServicio(stock: 5);

        var error = await Assert.ThrowsAsync<ReglaNegocioException>(() => servicio.RegistrarAsync(Venta((1, 10))));

        Assert.Contains("Paracetamol", error.Message);
        Assert.Contains("10", error.Message);
        Assert.Contains("5", error.Message);
        await _ventas.DidNotReceive().AgregarAsync(Arg.Any<VentaCab>(), Arg.Any<CancellationToken>());
        await _movimientos.DidNotReceive().RegistrarSalidaAsync(Arg.Any<int>(), Arg.Any<IReadOnlyList<MovimientoItemDto>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Sin_stock_avisa_que_no_hay_stock_del_producto()
    {
        var servicio = CrearServicio(stock: 0);

        var error = await Assert.ThrowsAsync<ReglaNegocioException>(() => servicio.RegistrarAsync(Venta((1, 1))));

        Assert.Contains("No hay stock", error.Message);
    }

    [Fact]
    public async Task Suma_las_cantidades_de_un_mismo_producto_antes_de_validar_el_stock()
    {
        var servicio = CrearServicio(stock: 5);

        // 3 + 4 = 7 unidades del mismo producto, con solo 5 en stock.
        await Assert.ThrowsAsync<ReglaNegocioException>(() => servicio.RegistrarAsync(Venta((1, 3), (1, 4))));
    }

    [Fact]
    public async Task Permite_vender_exactamente_el_stock_disponible()
    {
        var venta = await CrearServicio(stock: 4).RegistrarAsync(Venta((1, 4)));

        Assert.Equal(9, venta.IdVentaCab);
    }

    [Fact]
    public async Task Calcula_subtotal_igv_y_total_con_el_precio_de_venta()
    {
        var venta = await CrearServicio().RegistrarAsync(Venta((1, 2)));

        Assert.Equal(27m, venta.SubTotal);
        Assert.Equal(4.86m, venta.Igv);
        Assert.Equal(31.86m, venta.Total);

        var detalle = Assert.Single(venta.Detalles);
        Assert.Equal(13.5m, detalle.Precio);
        Assert.Equal(27m, detalle.SubTotal);
    }

    [Fact]
    public async Task Genera_el_movimiento_de_salida_de_la_venta()
    {
        await CrearServicio().RegistrarAsync(Venta((1, 2)));

        await _movimientos.Received(1).RegistrarSalidaAsync(
            9,
            Arg.Is<IReadOnlyList<MovimientoItemDto>>(i => i.Count == 1 && i[0].IdProducto == 1 && i[0].Cantidad == 2),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Con_un_producto_inexistente_responde_con_regla_de_negocio()
    {
        _productos.ObtenerAsync(99, Arg.Any<CancellationToken>()).Returns((ProductoDto?)null);

        var error = await Assert.ThrowsAsync<ReglaNegocioException>(() => CrearServicio().RegistrarAsync(Venta((99, 1))));

        Assert.Contains("99", error.Message);
    }

    [Fact]
    public async Task Si_falla_el_movimiento_deshace_la_venta()
    {
        _movimientos
            .RegistrarSalidaAsync(Arg.Any<int>(), Arg.Any<IReadOnlyList<MovimientoItemDto>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new HttpRequestException("Falló Movimientos.Service")));

        await Assert.ThrowsAsync<HttpRequestException>(() => CrearServicio().RegistrarAsync(Venta((1, 2))));

        await _ventas.Received(1).EliminarAsync(9, Arg.Any<CancellationToken>());
    }
}

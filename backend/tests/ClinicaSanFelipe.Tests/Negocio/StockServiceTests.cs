using Movimientos.Service.Application.Dtos;
using Movimientos.Service.Application.Interfaces;
using Movimientos.Service.Application.Services;
using Movimientos.Service.Domain.Entities;
using NSubstitute;

namespace ClinicaSanFelipe.Tests.Negocio;

// Regla: las Entradas suman y las Salidas restan.
public class StockServiceTests
{
    private readonly IMovimientoRepository _movimientos = Substitute.For<IMovimientoRepository>();

    private StockService CrearServicio() => new(_movimientos);

    private static readonly DateTime Fecha = new(2026, 11, 20, 10, 0, 0);

    [Fact]
    public async Task Las_entradas_suman_y_las_salidas_restan()
    {
        IReadOnlyList<MovimientoPlano> movimientos = new List<MovimientoPlano>
        {
            new(1, TipoMovimiento.Entrada, 10, Fecha),
            new(1, TipoMovimiento.Salida, 3, Fecha),
            new(1, TipoMovimiento.Entrada, 5, Fecha)
        };
        _movimientos.ListarPorProductoAsync(1, Arg.Any<CancellationToken>()).Returns(movimientos);

        var stock = await CrearServicio().ObtenerStockAsync(1);

        Assert.Equal(12, stock);
    }

    [Fact]
    public async Task Un_producto_sin_movimientos_tiene_stock_cero()
    {
        IReadOnlyList<MovimientoPlano> sinMovimientos = new List<MovimientoPlano>();
        _movimientos.ListarPorProductoAsync(2, Arg.Any<CancellationToken>()).Returns(sinMovimientos);

        var stock = await CrearServicio().ObtenerStockAsync(2);

        Assert.Equal(0, stock);
    }
}

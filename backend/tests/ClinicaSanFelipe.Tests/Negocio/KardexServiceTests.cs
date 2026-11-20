using Movimientos.Service.Application.Dtos;
using Movimientos.Service.Application.Interfaces;
using Movimientos.Service.Application.Services;
using Movimientos.Service.Domain.Entities;
using NSubstitute;

namespace ClinicaSanFelipe.Tests.Negocio;

// El Kardex tiene un renglón por producto con su stock actual.
public class KardexServiceTests
{
    private readonly IMovimientoRepository _movimientos = Substitute.For<IMovimientoRepository>();
    private readonly IProductosFacade _productos = Substitute.For<IProductosFacade>();

    private KardexService CrearServicio() => new(_movimientos, _productos);

    private static readonly DateTime Fecha = new(2026, 11, 20, 10, 0, 0);

    [Fact]
    public async Task Devuelve_un_renglon_por_producto_con_su_stock()
    {
        IReadOnlyList<ProductoDto> productos = new List<ProductoDto>
        {
            new(2, "Ibuprofeno", "L2", 12m, 16.2m),
            new(1, "Paracetamol", "L1", 10m, 13.5m)
        };
        IReadOnlyList<MovimientoPlano> movimientos = new List<MovimientoPlano>
        {
            new(1, TipoMovimiento.Entrada, 10, Fecha),
            new(1, TipoMovimiento.Salida, 3, Fecha)
        };
        _productos.ListarAsync(Arg.Any<CancellationToken>()).Returns(productos);
        _movimientos.ListarTodosAsync(Arg.Any<CancellationToken>()).Returns(movimientos);

        var kardex = await CrearServicio().ListarKardexAsync();

        Assert.Equal(2, kardex.Count);
        Assert.Equal(new[] { 1, 2 }, kardex.Select(k => k.IdProducto));

        var paracetamol = kardex[0];
        Assert.Equal("Paracetamol", paracetamol.NombreProducto);
        Assert.Equal(7, paracetamol.StockActual);
        Assert.Equal(10m, paracetamol.Costo);
        Assert.Equal(13.5m, paracetamol.PrecioVenta);

        Assert.Equal(0, kardex[1].StockActual);
    }
}

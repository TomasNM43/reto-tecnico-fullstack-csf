namespace Movimientos.Service.Domain.Entities;

public class MovimientoCab
{
    public int IdMovimientoCab { get; set; }
    public DateTime FecRegistro { get; set; }
    public TipoMovimiento IdTipoMovimiento { get; set; }

    // Id de la compra (Entrada) o de la venta (Salida).
    public int IdDocumentoOrigen { get; set; }

    public List<MovimientoDet> Detalles { get; set; } = new();
}

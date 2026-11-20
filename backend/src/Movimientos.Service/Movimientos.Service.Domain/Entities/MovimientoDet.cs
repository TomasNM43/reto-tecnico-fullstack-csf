namespace Movimientos.Service.Domain.Entities;

public class MovimientoDet
{
    public int IdMovimientoDet { get; set; }
    public int IdMovimientoCab { get; set; }
    public int IdProducto { get; set; }
    public int Cantidad { get; set; }
}

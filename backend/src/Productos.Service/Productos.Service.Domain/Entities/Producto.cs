namespace Productos.Service.Domain.Entities;

public class Producto
{
    public int IdProducto { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public string NroLote { get; set; } = string.Empty;
    public DateTime FecRegistro { get; set; }
    public decimal Costo { get; set; }
    public decimal PrecioVenta { get; set; }
}

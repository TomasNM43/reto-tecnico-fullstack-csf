using System.ComponentModel.DataAnnotations;

namespace Ventas.Service.Application.Dtos;

public class RegistrarVentaRequest
{
    [Required(ErrorMessage = "La venta debe tener al menos un producto.")]
    [MinLength(1, ErrorMessage = "La venta debe tener al menos un producto.")]
    public List<VentaItemRequest> Detalles { get; set; } = new();
}

// El precio no viaja en la petición: se toma del PrecioVenta del producto.
public class VentaItemRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "El producto es obligatorio.")]
    public int IdProducto { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
    public int Cantidad { get; set; }
}

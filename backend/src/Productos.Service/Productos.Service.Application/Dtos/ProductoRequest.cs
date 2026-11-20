using System.ComponentModel.DataAnnotations;

namespace Productos.Service.Application.Dtos;

// Se usa tanto para registrar como para actualizar un producto.
public class ProductoRequest
{
    [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
    [StringLength(150)]
    public string NombreProducto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El número de lote es obligatorio.")]
    [StringLength(50)]
    public string NroLote { get; set; } = string.Empty;

    [Range(0, 999999999.99, ErrorMessage = "El costo no puede ser negativo.")]
    public decimal Costo { get; set; }

    [Range(0, 999999999.99, ErrorMessage = "El precio de venta no puede ser negativo.")]
    public decimal PrecioVenta { get; set; }
}

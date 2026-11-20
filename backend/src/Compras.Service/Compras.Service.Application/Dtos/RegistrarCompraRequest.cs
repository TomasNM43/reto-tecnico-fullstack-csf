using System.ComponentModel.DataAnnotations;

namespace Compras.Service.Application.Dtos;

public class RegistrarCompraRequest
{
    [Required(ErrorMessage = "La compra debe tener al menos un producto.")]
    [MinLength(1, ErrorMessage = "La compra debe tener al menos un producto.")]
    public List<CompraItemRequest> Detalles { get; set; } = new();
}

public class CompraItemRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "El producto es obligatorio.")]
    public int IdProducto { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
    public int Cantidad { get; set; }

    // Precio unitario de compra (se convierte en el nuevo Costo del producto).
    [Range(0.01, 999999999.99, ErrorMessage = "El precio debe ser mayor a cero.")]
    public decimal Precio { get; set; }
}

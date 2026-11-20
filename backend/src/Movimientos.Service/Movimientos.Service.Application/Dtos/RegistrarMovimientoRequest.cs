using System.ComponentModel.DataAnnotations;
using Movimientos.Service.Domain.Entities;

namespace Movimientos.Service.Application.Dtos;

// Lo envían Compras (Entrada) y Ventas (Salida) para dejar registrado el movimiento.
public class RegistrarMovimientoRequest
{
    [Required]
    [EnumDataType(typeof(TipoMovimiento), ErrorMessage = "El tipo de movimiento debe ser 1 (Entrada) o 2 (Salida).")]
    public TipoMovimiento IdTipoMovimiento { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "El documento de origen es obligatorio.")]
    public int IdDocumentoOrigen { get; set; }

    [Required(ErrorMessage = "El movimiento debe tener al menos un producto.")]
    [MinLength(1, ErrorMessage = "El movimiento debe tener al menos un producto.")]
    public List<MovimientoItemRequest> Detalles { get; set; } = new();
}

public class MovimientoItemRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "El producto es obligatorio.")]
    public int IdProducto { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
    public int Cantidad { get; set; }
}

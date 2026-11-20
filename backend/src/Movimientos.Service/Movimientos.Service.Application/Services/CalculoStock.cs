using Movimientos.Service.Application.Dtos;
using Movimientos.Service.Domain.Entities;

namespace Movimientos.Service.Application.Services;

// Regla del stock: las Entradas suman y las Salidas restan.
internal static class CalculoStock
{
    public static int Calcular(IEnumerable<MovimientoPlano> movimientos)
        => movimientos.Sum(m => m.Tipo == TipoMovimiento.Entrada ? m.Cantidad : -m.Cantidad);
}

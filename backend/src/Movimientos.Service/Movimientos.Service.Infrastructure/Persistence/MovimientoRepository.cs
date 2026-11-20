using Microsoft.EntityFrameworkCore;
using Movimientos.Service.Application.Dtos;
using Movimientos.Service.Application.Interfaces;
using Movimientos.Service.Domain.Entities;

namespace Movimientos.Service.Infrastructure.Persistence;

public class MovimientoRepository : IMovimientoRepository
{
    private readonly MovimientosDbContext _db;

    public MovimientoRepository(MovimientosDbContext db) => _db = db;

    public async Task AgregarAsync(MovimientoCab movimiento, CancellationToken ct = default)
    {
        // Un solo SaveChanges: cabecera y detalle se guardan juntos o no se guarda nada.
        _db.MovimientosCab.Add(movimiento);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<MovimientoPlano>> ListarPorProductoAsync(int idProducto, CancellationToken ct = default)
        => await ConsultaPlana(idProducto).ToListAsync(ct);

    public async Task<IReadOnlyList<MovimientoPlano>> ListarTodosAsync(CancellationToken ct = default)
        => await ConsultaPlana(null).ToListAsync(ct);

    // El filtro y el orden van antes del select porque EF no los traduce sobre el record ya proyectado.
    private IQueryable<MovimientoPlano> ConsultaPlana(int? idProducto)
    {
        var detalles = _db.MovimientosDet.AsNoTracking().AsQueryable();

        if (idProducto.HasValue)
            detalles = detalles.Where(d => d.IdProducto == idProducto.Value);

        return from d in detalles
               join c in _db.MovimientosCab.AsNoTracking() on d.IdMovimientoCab equals c.IdMovimientoCab
               orderby c.FecRegistro, d.IdMovimientoDet
               select new MovimientoPlano(d.IdProducto, c.IdTipoMovimiento, d.Cantidad, c.FecRegistro);
    }
}

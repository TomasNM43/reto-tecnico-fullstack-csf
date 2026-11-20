using Microsoft.EntityFrameworkCore;
using Ventas.Service.Application.Interfaces;
using Ventas.Service.Domain.Entities;

namespace Ventas.Service.Infrastructure.Persistence;

public class VentaRepository : IVentaRepository
{
    private readonly VentasDbContext _db;

    public VentaRepository(VentasDbContext db) => _db = db;

    public async Task AgregarAsync(VentaCab venta, CancellationToken ct = default)
    {
        // Un solo SaveChanges: cabecera y detalle se guardan juntos o no se guarda nada.
        _db.VentasCab.Add(venta);
        await _db.SaveChangesAsync(ct);
    }

    public async Task EliminarAsync(int idVentaCab, CancellationToken ct = default)
    {
        // El detalle se borra en cascada desde la base de datos.
        await _db.VentasCab.Where(v => v.IdVentaCab == idVentaCab).ExecuteDeleteAsync(ct);
    }

    public async Task<IReadOnlyList<VentaCab>> ListarAsync(CancellationToken ct = default)
        => await _db.VentasCab
            .AsNoTracking()
            .Include(v => v.Detalles)
            .OrderByDescending(v => v.IdVentaCab)
            .ToListAsync(ct);
}

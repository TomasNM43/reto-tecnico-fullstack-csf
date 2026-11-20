using Compras.Service.Application.Interfaces;
using Compras.Service.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Compras.Service.Infrastructure.Persistence;

public class CompraRepository : ICompraRepository
{
    private readonly ComprasDbContext _db;

    public CompraRepository(ComprasDbContext db) => _db = db;

    public async Task AgregarAsync(CompraCab compra, CancellationToken ct = default)
    {
        // Un solo SaveChanges: cabecera y detalle se guardan juntos o no se guarda nada.
        _db.ComprasCab.Add(compra);
        await _db.SaveChangesAsync(ct);
    }

    public async Task EliminarAsync(int idCompraCab, CancellationToken ct = default)
    {
        // El detalle se borra en cascada desde la base de datos.
        await _db.ComprasCab.Where(c => c.IdCompraCab == idCompraCab).ExecuteDeleteAsync(ct);
    }

    public async Task<IReadOnlyList<CompraCab>> ListarAsync(CancellationToken ct = default)
        => await _db.ComprasCab
            .AsNoTracking()
            .Include(c => c.Detalles)
            .OrderByDescending(c => c.IdCompraCab)
            .ToListAsync(ct);
}

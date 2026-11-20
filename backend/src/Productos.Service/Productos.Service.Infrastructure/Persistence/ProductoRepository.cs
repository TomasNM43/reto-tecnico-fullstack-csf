using Microsoft.EntityFrameworkCore;
using Productos.Service.Application.Interfaces;
using Productos.Service.Domain.Entities;

namespace Productos.Service.Infrastructure.Persistence;

public class ProductoRepository : IProductoRepository
{
    private readonly ProductosDbContext _db;

    public ProductoRepository(ProductosDbContext db) => _db = db;

    public async Task<IReadOnlyList<Producto>> ListarAsync(CancellationToken ct = default)
        => await _db.Productos.AsNoTracking().OrderBy(p => p.IdProducto).ToListAsync(ct);

    public Task<Producto?> ObtenerPorIdAsync(int idProducto, CancellationToken ct = default)
        => _db.Productos.FirstOrDefaultAsync(p => p.IdProducto == idProducto, ct);

    public async Task AgregarAsync(Producto producto, CancellationToken ct = default)
    {
        _db.Productos.Add(producto);
        await _db.SaveChangesAsync(ct);
    }

    public async Task ActualizarAsync(Producto producto, CancellationToken ct = default)
    {
        // El producto ya viene rastreado por el contexto, así que solo se guardan los campos que cambiaron.
        await _db.SaveChangesAsync(ct);
    }
}

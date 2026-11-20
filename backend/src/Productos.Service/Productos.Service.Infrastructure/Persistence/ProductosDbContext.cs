using Microsoft.EntityFrameworkCore;
using Productos.Service.Domain.Entities;

namespace Productos.Service.Infrastructure.Persistence;

public class ProductosDbContext : DbContext
{
    public ProductosDbContext(DbContextOptions<ProductosDbContext> options) : base(options) { }

    // Este servicio solo ve la tabla productos.
    public DbSet<Producto> Productos => Set<Producto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>(e =>
        {
            e.ToTable("productos");
            e.HasKey(p => p.IdProducto);
            e.Property(p => p.IdProducto).HasColumnName("id_producto").ValueGeneratedOnAdd();
            e.Property(p => p.NombreProducto).HasColumnName("nombre_producto");
            e.Property(p => p.NroLote).HasColumnName("nrolote");
            // La fecha la pone la base de datos (DEFAULT NOW()). La columna es "timestamp" sin zona horaria.
            e.Property(p => p.FecRegistro).HasColumnName("fec_registro").HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("NOW()").ValueGeneratedOnAdd();
            e.Property(p => p.Costo).HasColumnName("costo").HasPrecision(18, 2);
            e.Property(p => p.PrecioVenta).HasColumnName("precioventa").HasPrecision(18, 2);
        });
    }
}

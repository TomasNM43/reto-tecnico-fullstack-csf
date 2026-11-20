using Compras.Service.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Compras.Service.Infrastructure.Persistence;

public class ComprasDbContext : DbContext
{
    public ComprasDbContext(DbContextOptions<ComprasDbContext> options) : base(options) { }

    // Este servicio solo ve las tablas de compras.
    public DbSet<CompraCab> ComprasCab => Set<CompraCab>();
    public DbSet<CompraDet> ComprasDet => Set<CompraDet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CompraCab>(e =>
        {
            e.ToTable("compracab");
            e.HasKey(c => c.IdCompraCab);
            e.Property(c => c.IdCompraCab).HasColumnName("id_compracab").ValueGeneratedOnAdd();
            // La fecha la pone la base de datos (DEFAULT NOW()). La columna es "timestamp" sin zona horaria.
            e.Property(c => c.FecRegistro).HasColumnName("fecregistro").HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("NOW()").ValueGeneratedOnAdd();
            e.Property(c => c.SubTotal).HasColumnName("subtotal").HasPrecision(18, 2);
            e.Property(c => c.Igv).HasColumnName("igv").HasPrecision(18, 2);
            e.Property(c => c.Total).HasColumnName("total").HasPrecision(18, 2);
            e.HasMany(c => c.Detalles).WithOne().HasForeignKey(d => d.IdCompraCab);
        });

        modelBuilder.Entity<CompraDet>(e =>
        {
            e.ToTable("compradet");
            e.HasKey(d => d.IdCompraDet);
            e.Property(d => d.IdCompraDet).HasColumnName("id_compradet").ValueGeneratedOnAdd();
            e.Property(d => d.IdCompraCab).HasColumnName("id_compracab");
            e.Property(d => d.IdProducto).HasColumnName("id_producto");
            e.Property(d => d.Cantidad).HasColumnName("cantidad");
            e.Property(d => d.Precio).HasColumnName("precio").HasPrecision(18, 2);
            e.Property(d => d.SubTotal).HasColumnName("sub_total").HasPrecision(18, 2);
            e.Property(d => d.Igv).HasColumnName("igv").HasPrecision(18, 2);
            e.Property(d => d.Total).HasColumnName("total").HasPrecision(18, 2);
        });
    }
}

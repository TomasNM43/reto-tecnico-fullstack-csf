using Microsoft.EntityFrameworkCore;
using Ventas.Service.Domain.Entities;

namespace Ventas.Service.Infrastructure.Persistence;

public class VentasDbContext : DbContext
{
    public VentasDbContext(DbContextOptions<VentasDbContext> options) : base(options) { }

    // Este servicio solo ve las tablas de ventas.
    public DbSet<VentaCab> VentasCab => Set<VentaCab>();
    public DbSet<VentaDet> VentasDet => Set<VentaDet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VentaCab>(e =>
        {
            e.ToTable("ventacab");
            e.HasKey(v => v.IdVentaCab);
            e.Property(v => v.IdVentaCab).HasColumnName("id_ventacab").ValueGeneratedOnAdd();
            // La fecha la pone la base de datos (DEFAULT NOW()). La columna es "timestamp" sin zona horaria.
            e.Property(v => v.FecRegistro).HasColumnName("fecregistro").HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("NOW()").ValueGeneratedOnAdd();
            e.Property(v => v.SubTotal).HasColumnName("subtotal").HasPrecision(18, 2);
            e.Property(v => v.Igv).HasColumnName("igv").HasPrecision(18, 2);
            e.Property(v => v.Total).HasColumnName("total").HasPrecision(18, 2);
            e.HasMany(v => v.Detalles).WithOne().HasForeignKey(d => d.IdVentaCab);
        });

        modelBuilder.Entity<VentaDet>(e =>
        {
            e.ToTable("ventadet");
            e.HasKey(d => d.IdVentaDet);
            e.Property(d => d.IdVentaDet).HasColumnName("id_ventadet").ValueGeneratedOnAdd();
            e.Property(d => d.IdVentaCab).HasColumnName("id_ventacab");
            e.Property(d => d.IdProducto).HasColumnName("id_producto");
            e.Property(d => d.Cantidad).HasColumnName("cantidad");
            e.Property(d => d.Precio).HasColumnName("precio").HasPrecision(18, 2);
            e.Property(d => d.SubTotal).HasColumnName("sub_total").HasPrecision(18, 2);
            e.Property(d => d.Igv).HasColumnName("igv").HasPrecision(18, 2);
            e.Property(d => d.Total).HasColumnName("total").HasPrecision(18, 2);
        });
    }
}

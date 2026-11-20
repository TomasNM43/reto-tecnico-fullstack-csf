using Microsoft.EntityFrameworkCore;
using Movimientos.Service.Domain.Entities;

namespace Movimientos.Service.Infrastructure.Persistence;

public class MovimientosDbContext : DbContext
{
    public MovimientosDbContext(DbContextOptions<MovimientosDbContext> options) : base(options) { }

    // Este servicio solo ve las tablas de movimientos.
    public DbSet<MovimientoCab> MovimientosCab => Set<MovimientoCab>();
    public DbSet<MovimientoDet> MovimientosDet => Set<MovimientoDet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MovimientoCab>(e =>
        {
            e.ToTable("movimientocab");
            e.HasKey(m => m.IdMovimientoCab);
            e.Property(m => m.IdMovimientoCab).HasColumnName("id_movimientocab").ValueGeneratedOnAdd();
            // La fecha la pone la base de datos (DEFAULT NOW()). La columna es "timestamp" sin zona horaria.
            e.Property(m => m.FecRegistro).HasColumnName("fec_registro").HasColumnType("timestamp without time zone")
                .HasDefaultValueSql("NOW()").ValueGeneratedOnAdd();
            e.Property(m => m.IdTipoMovimiento).HasColumnName("id_tipomovimiento").HasConversion<short>();
            e.Property(m => m.IdDocumentoOrigen).HasColumnName("id_documentoorigen");
            e.HasMany(m => m.Detalles).WithOne().HasForeignKey(d => d.IdMovimientoCab);
        });

        modelBuilder.Entity<MovimientoDet>(e =>
        {
            e.ToTable("movimientodet");
            e.HasKey(d => d.IdMovimientoDet);
            e.Property(d => d.IdMovimientoDet).HasColumnName("id_movimientodet").ValueGeneratedOnAdd();
            e.Property(d => d.IdMovimientoCab).HasColumnName("id_movimientocab");
            e.Property(d => d.IdProducto).HasColumnName("id_producto");
            e.Property(d => d.Cantidad).HasColumnName("cantidad");
        });
    }
}

using Auth.Service.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth.Service.Infrastructure.Persistence;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    // Este servicio solo ve la tabla usuarios.
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(e =>
        {
            e.ToTable("usuarios");
            e.HasKey(u => u.Id);
            e.Property(u => u.Id).HasColumnName("id");
            e.Property(u => u.Username).HasColumnName("username");
            e.Property(u => u.PasswordHash).HasColumnName("passwordhash");
            e.Property(u => u.Rol).HasColumnName("rol");
        });
    }
}

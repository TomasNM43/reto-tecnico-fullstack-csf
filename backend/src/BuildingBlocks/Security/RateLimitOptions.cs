namespace BuildingBlocks.Security;

// Límites de peticiones. Sección "RateLimit" del appsettings.
public class RateLimitOptions
{
    public const string SectionName = "RateLimit";

    // Nombre de la política estricta para el login (se usa con [EnableRateLimiting]).
    public const string LoginPolicy = "login";

    public int PermitLimit { get; set; } = 100;
    public int WindowSeconds { get; set; } = 60;

    // El login es más estricto para frenar ataques de fuerza bruta.
    public int LoginPermitLimit { get; set; } = 5;
    public int LoginWindowSeconds { get; set; } = 60;
}

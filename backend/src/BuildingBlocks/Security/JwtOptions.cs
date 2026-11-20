namespace BuildingBlocks.Security;

// Configuración del JWT. Se lee de la sección "Jwt" del appsettings.
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "ClinicaSanFelipe";
    public string Audience { get; set; } = "ClinicaSanFelipe.Clients";

    // El reto pide que el token expire a los 30 minutos.
    public int ExpirationMinutes { get; set; } = 30;
}

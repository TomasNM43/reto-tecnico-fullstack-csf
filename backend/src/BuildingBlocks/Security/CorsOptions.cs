namespace BuildingBlocks.Security;

// Orígenes permitidos por CORS (solo el frontend). Sección "Cors" del appsettings.
public class CorsOptions
{
    public const string SectionName = "Cors";
    public const string PolicyName = "FrontendOnly";

    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
}

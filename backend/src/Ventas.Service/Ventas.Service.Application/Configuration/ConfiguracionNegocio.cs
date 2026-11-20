namespace Ventas.Service.Application.Configuration;

// Valores del negocio que se pueden cambiar en el appsettings (sección "Negocio").
public class ConfiguracionNegocio
{
    public const string SectionName = "Negocio";

    // Tasa del IGV. Se aplica sobre el subtotal.
    public decimal IgvRate { get; set; } = 0.18m;
}

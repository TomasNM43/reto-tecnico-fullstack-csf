namespace Compras.Service.Application.Configuration;

// Valores del negocio que se pueden cambiar en el appsettings (sección "Negocio").
public class ConfiguracionNegocio
{
    public const string SectionName = "Negocio";

    // Tasa del IGV. Se aplica sobre el subtotal.
    public decimal IgvRate { get; set; } = 0.18m;

    // Se multiplica por el costo para obtener el precio de venta (Costo × 1.35).
    public decimal FactorPrecioVenta { get; set; } = 1.35m;
}

using BuildingBlocks;
using Ventas.Service.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Registro de servicios en el contenedor de dependencias.
builder.Services.AddControllers();
builder.Services.AddVentasServices(builder.Configuration);
builder.AddBuildingBlocks("Ventas Service");

var app = builder.Build();

// Configuración del pipeline de peticiones HTTP.
app.UseBuildingBlocks();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { servicio = "Ventas", estado = "ok" })).AllowAnonymous();

app.Run();

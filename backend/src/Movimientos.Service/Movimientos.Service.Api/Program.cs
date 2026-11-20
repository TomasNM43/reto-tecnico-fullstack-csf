using BuildingBlocks;
using Movimientos.Service.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Registro de servicios en el contenedor de dependencias.
builder.Services.AddControllers();
builder.Services.AddMovimientosServices(builder.Configuration);
builder.AddBuildingBlocks("Movimientos Service");

var app = builder.Build();

// Configuración del pipeline de peticiones HTTP.
app.UseBuildingBlocks();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { servicio = "Movimientos", estado = "ok" })).AllowAnonymous();

app.Run();

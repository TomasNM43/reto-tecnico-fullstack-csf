using BuildingBlocks;
using Compras.Service.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Registro de servicios en el contenedor de dependencias.
builder.Services.AddControllers();
builder.Services.AddComprasServices(builder.Configuration);
builder.AddBuildingBlocks("Compras Service");

var app = builder.Build();

// Configuración del pipeline de peticiones HTTP.
app.UseBuildingBlocks();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { servicio = "Compras", estado = "ok" })).AllowAnonymous();

app.Run();

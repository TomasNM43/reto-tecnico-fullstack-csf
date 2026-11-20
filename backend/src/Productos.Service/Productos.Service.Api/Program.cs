using BuildingBlocks;
using Productos.Service.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Registro de servicios en el contenedor de dependencias.
builder.Services.AddControllers();
builder.Services.AddProductosServices(builder.Configuration);
builder.AddBuildingBlocks("Productos Service");

var app = builder.Build();

// Configuración del pipeline de peticiones HTTP.
app.UseBuildingBlocks();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { servicio = "Productos", estado = "ok" })).AllowAnonymous();

app.Run();

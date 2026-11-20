using BuildingBlocks;
using Auth.Service.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Registro de servicios en el contenedor de dependencias.
builder.Services.AddControllers();
builder.Services.AddAuthServices(builder.Configuration);
builder.AddBuildingBlocks("Auth Service");

var app = builder.Build();

// Configuración del pipeline de peticiones HTTP.
app.UseBuildingBlocks();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { servicio = "Auth", estado = "ok" })).AllowAnonymous();

app.Run();

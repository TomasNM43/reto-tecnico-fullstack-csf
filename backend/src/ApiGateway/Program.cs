using BuildingBlocks;

var builder = WebApplication.CreateBuilder(args);

// Registro de servicios en el contenedor de dependencias.
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// JWT, rate limit, CORS y Swagger compartidos con el resto de servicios.
builder.AddBuildingBlocks("API Gateway");

var app = builder.Build();

// Configuración del pipeline de peticiones HTTP.
app.UseBuildingBlocks();

app.MapGet("/health", () => Results.Ok(new { servicio = "ApiGateway", estado = "ok" })).AllowAnonymous();

// Todo lo demás se reenvía al microservicio que corresponda según las rutas del appsettings.
app.MapReverseProxy();

app.Run();

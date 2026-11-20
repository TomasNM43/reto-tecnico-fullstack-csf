using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace BuildingBlocks.Http;

// Reenvía el token JWT de la petición actual a la llamada hacia otro microservicio.
public class BearerTokenForwardingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _contexto;

    public BearerTokenForwardingHandler(IHttpContextAccessor contexto) => _contexto = contexto;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var autorizacion = _contexto.HttpContext?.Request.Headers.Authorization.ToString();

        if (!string.IsNullOrWhiteSpace(autorizacion) && AuthenticationHeaderValue.TryParse(autorizacion, out var valor))
            request.Headers.Authorization = valor;

        return base.SendAsync(request, ct);
    }
}

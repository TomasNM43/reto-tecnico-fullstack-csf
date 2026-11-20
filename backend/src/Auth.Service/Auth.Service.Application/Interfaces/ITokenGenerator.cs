using Auth.Service.Domain.Entities;

namespace Auth.Service.Application.Interfaces;

public interface ITokenGenerator
{
    // Devuelve el token y la fecha en que expira.
    (string Token, DateTime ExpiraEnUtc) Generar(Usuario usuario);
}

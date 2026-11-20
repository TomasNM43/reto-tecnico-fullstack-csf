namespace Auth.Service.Application.Dtos;

public record LoginResponse(string Token, DateTime ExpiraEnUtc, string Username, string Rol);

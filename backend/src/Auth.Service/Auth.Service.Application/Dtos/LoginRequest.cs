using System.ComponentModel.DataAnnotations;

namespace Auth.Service.Application.Dtos;

public class LoginRequest
{
    [Required(ErrorMessage = "El usuario es obligatorio.")]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100)]
    public string Password { get; set; } = string.Empty;
}

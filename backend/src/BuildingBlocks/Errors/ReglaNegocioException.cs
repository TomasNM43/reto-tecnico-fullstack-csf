namespace BuildingBlocks.Errors;

// Se lanza cuando se incumple una regla de negocio (por ejemplo, stock insuficiente).
// El middleware global la convierte en una respuesta 400 con el mensaje tal cual.
public class ReglaNegocioException : Exception
{
    public ReglaNegocioException(string mensaje) : base(mensaje) { }
}

namespace Ventas.Service.Application.Dtos;

public record VentaResponse(
    int IdVentaCab,
    DateTime FecRegistro,
    decimal SubTotal,
    decimal Igv,
    decimal Total,
    IReadOnlyList<VentaDetalleResponse> Detalles);

public record VentaDetalleResponse(
    int IdVentaDet,
    int IdProducto,
    int Cantidad,
    decimal Precio,
    decimal SubTotal,
    decimal Igv,
    decimal Total);
